using System.Globalization;
using Fractions;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public static class TransactionExtensions
{
    public static Transaction AddTransaction(this KmyMoneyFileRoot kmyMoneyFileRoot,
        string from,
        string to,
        decimal amount,
        string currency,
        string? memo)
    {
        var fromAccount = kmyMoneyFileRoot.Accounts.GetByNameOrId(from);
        var toAccount = kmyMoneyFileRoot.Accounts.GetByNameOrId(to);

        if (fromAccount == null)
        {
            throw new($"Source account '{from}' not found.");
        }

        if (toAccount == null)
        {
            throw new($"Destination account '{to}' not found.");
        }

        var postDate = DateTime.Now;
        var transactionId = kmyMoneyFileRoot.GenerateNextTransactionId();

        var fromAmount = Fraction.FromDecimal(-amount);
        var fromAmountConverted = fromAmount;
        var toAmount = Fraction.FromDecimal(amount);
        var toAmountConverted = toAmount;

        // Handle currency conversion
        if (fromAccount.Currency != currency)
        {
            fromAmountConverted = kmyMoneyFileRoot.Prices.ConvertCurrency(-amount, currency, fromAccount.Currency);
        }

        if (toAccount.Currency != currency)
        {
            toAmountConverted = kmyMoneyFileRoot.Prices.ConvertCurrency(amount, currency, toAccount.Currency);
        }

        var transaction = new Transaction
        {
            Id = transactionId,
            PostDate = postDate.ToString("yyyy-MM-dd"),
            EntryDate = postDate.ToString("yyyy-MM-dd"),
            Commodity = currency,
            Memo = memo ?? string.Empty,
            Splits = new()
            {
                Split =
                [
                    new()
                    {
                        Id = "S0001",
                        Account = fromAccount.Id,
                        Value = fromAmount.ToString(),
                        Shares = fromAmountConverted.ToString(),
                        Memo = memo ?? string.Empty
                    },
                    new()
                    {
                        Id = "S0002",
                        Account = toAccount.Id,
                        Value = toAmount.ToString(),
                        Shares = toAmountConverted.ToString(),
                        Memo = memo ?? string.Empty
                    }
                ]
            }
        };

        var transactions = kmyMoneyFileRoot.Transactions.Values.ToList();
        transactions.Add(transaction);
        kmyMoneyFileRoot.Transactions.Values = transactions.ToArray();

        return transaction;
    }

    private static string GenerateNextTransactionId(this KmyMoneyFileRoot kmyMoneyFileRoot)
    {
        var maxId = kmyMoneyFileRoot.Transactions.Values.Select(t => int.Parse(t.Id[1..]))
            .DefaultIfEmpty(0)
            .Max();
        return $"T{maxId + 1:D18}";
    }

    public static IDictionary<string, DateTimeOffset>
        GetLatestTransactionsByAccountId(this Transactions transactions) =>
        transactions.Values
            .SelectMany(transaction =>
                transaction.Splits.Split
                    .Select(s => (AccountId: s.Account, TransactionDate: transaction.EntryDate)))
            .GroupBy(account => account.AccountId)
            .Select(kv =>
                (AccountId: kv.Key,
                    LatestTransaction: kv.Select(v => DateTimeOffset.ParseExact(
                            v.TransactionDate, ["yyyy-MM-dd"], CultureInfo.InvariantCulture))
                        .OrderDescending()
                        .First()))
            .OrderByDescending(t => t.LatestTransaction)
            .ToDictionary(t => t.AccountId, t => t.LatestTransaction);
}