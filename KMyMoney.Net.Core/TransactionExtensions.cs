using Fractions;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public static class TransactionExtensions
{
    public static Transaction AddTransaction(this KmyMoneyFile kmyMoneyFile,
        string from,
        string to,
        decimal amount,
        string currency,
        string? memo)
    {
        var fromAccount = kmyMoneyFile.Accounts.GetByNameOrId(from);
        var toAccount = kmyMoneyFile.Accounts.GetByNameOrId(to);

        if (fromAccount == null)
        {
            throw new($"Source account '{from}' not found.");
        }

        if (toAccount == null)
        {
            throw new($"Destination account '{to}' not found.");
        }

        var postDate = DateTime.Now;
        var transactionId = kmyMoneyFile.GenerateNextTransactionId();

        var fromAmount = Fraction.FromDecimal(-amount);
        var fromAmountConverted =  fromAmount; 
        var toAmount = Fraction.FromDecimal(amount);
        var toAmountConverted = toAmount;

        // Handle currency conversion
        if (fromAccount.Currency != currency)
        {
            fromAmountConverted = kmyMoneyFile.Prices.ConvertCurrency(amount, currency, fromAccount.Currency);
        }

        if (toAccount.Currency != currency)
        {
            toAmountConverted = kmyMoneyFile.Prices.ConvertCurrency(amount, currency, toAccount.Currency);
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

        var transactions = kmyMoneyFile.Transactions.Values.ToList();
        transactions.Add(transaction);
        kmyMoneyFile.Transactions.Values = transactions.ToArray();

        return transaction;
    }
    
    public static string GenerateNextTransactionId(this KmyMoneyFile kmyMoneyFile)
    {
        var maxId = kmyMoneyFile.Transactions.Values.Select(t => int.Parse(t.Id[1..]))
            .DefaultIfEmpty(0)
            .Max();
        return $"T{maxId + 1:D18}";
    }
}