using Fractions;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public class TransactionRepository(KmyMoneyFile kmyMoneyFile)
{
    public Transaction AddTransaction(Account fromAccount, Account toAccount, decimal amount, string currency, string? memo)
    {
        var postDate = DateTime.Now;
        var transactionId = GenerateNextTransactionId();

        var fromAmount = Fraction.FromDecimal(-amount);
        var fromAmountConverted =  fromAmount; 
        var toAmount = Fraction.FromDecimal(amount);
        var toAmountConverted = toAmount;

        // Handle currency conversion
        if (fromAccount.Currency != currency)
        {
            fromAmountConverted = ConvertCurrency(amount, currency, fromAccount.Currency);
        }

        if (toAccount.Currency != currency)
        {
            toAmountConverted = ConvertCurrency(amount, currency, toAccount.Currency);
        }

        var transaction = new Transaction
        {
            Id = transactionId,
            PostDate = postDate.ToString("yyyy-MM-dd"),
            EntryDate = postDate.ToString("yyyy-MM-dd"),
            Commodity = currency,
            Memo = memo ?? string.Empty,
            Splits = new Splits
            {
                Split =
                [
                    new Split
                    {
                        Id = "S0001",
                        Account = fromAccount.Id,
                        Value = fromAmount.ToString(),
                        Shares = fromAmountConverted.ToString(),
                        Memo = memo ?? string.Empty
                    },
                    new Split
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

    private string GenerateNextTransactionId()
    {
        var maxId = kmyMoneyFile.Transactions.Values.Select(t => int.Parse(t.Id.Substring(1)))
            .DefaultIfEmpty(0)
            .Max();
        return $"T{maxId + 1:D18}";
    }

    private Fraction ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        var pricePair = kmyMoneyFile.Prices.Values.FirstOrDefault(p => p.From == fromCurrency && p.To == toCurrency);
        if (pricePair == null)
        {
            // Also check reverse pair
            pricePair = kmyMoneyFile.Prices.Values.FirstOrDefault(p => p.From == toCurrency && p.To == fromCurrency);
            if (pricePair != null)
            {
                var latestPrice = pricePair.Price.OrderByDescending(p => p.Date).First();
                var price = Fraction.FromString(latestPrice.Price);
                return amount / price;
            }
            throw new Exception($"No exchange rate found for {fromCurrency} to {toCurrency}");
        }
        else
        {
            var latestPrice = pricePair.Price.OrderByDescending(p => p.Date).First();
            var price = Fraction.FromString(latestPrice.Price);
            return amount * price;
        }
    }
}