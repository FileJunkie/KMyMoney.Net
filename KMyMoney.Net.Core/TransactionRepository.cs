using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core
{
    public class TransactionRepository(KmyMoneyFile kmyMoneyFile)
    {
        public Transaction AddTransaction(Account fromAccount, Account toAccount, decimal amount, string currency, string? memo)
        {
            var postDate = DateTime.Now;
            var transactionId = GenerateNextTransactionId();

            var fromAmount = -amount;
            var toAmount = amount;

            // Handle currency conversion
            if (fromAccount.Currency != currency)
            {
                fromAmount = ConvertCurrency(amount, currency, fromAccount.Currency, postDate);
            }

            if (toAccount.Currency != currency)
            {
                toAmount = ConvertCurrency(amount, currency, toAccount.Currency, postDate);
            }

            var transaction = new Transaction
            {
                Id = transactionId,
                PostDate = postDate.ToString("yyyy-MM-dd"),
                EntryDate = postDate.ToString("yyyy-MM-dd"),
                Commodity = currency,
                Memo = memo,
                Splits = new Splits
                {
                    Split =
                    [
                        new Split
                        {
                            Id = $"S0001",
                            Account = fromAccount.Id,
                            Value = FormatAmount(fromAmount),
                            Shares = FormatAmount(fromAmount),
                            Memo = memo ?? string.Empty
                        },
                        new Split
                        {
                            Id = $"S0002",
                            Account = toAccount.Id,
                            Value = FormatAmount(toAmount),
                            Shares = FormatAmount(toAmount),
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

        private static string FormatAmount(decimal amount)
        {
            return $"{(long)(amount * 100)}/100";
        }

        private decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, DateTime date)
        {
            if (kmyMoneyFile.Prices == null)
            {
                throw new Exception("No price information available in the file.");
            }

            var pricePair = kmyMoneyFile.Prices.Values.FirstOrDefault(p => p.From == fromCurrency && p.To == toCurrency);
            if (pricePair == null)
            {
                // Also check reverse pair
                pricePair = kmyMoneyFile.Prices.Values.FirstOrDefault(p => p.From == toCurrency && p.To == fromCurrency);
                if (pricePair != null)
                {
                    var latestPrice = pricePair.Price.OrderByDescending(p => p.Date).First();
                    var price = decimal.Parse(latestPrice.Price.Split('/')[0]) / decimal.Parse(latestPrice.Price.Split('/')[1]);
                    return amount / price;
                }
                throw new Exception($"No exchange rate found for {fromCurrency} to {toCurrency}");
            }
            else
            {
                var latestPrice = pricePair.Price.OrderByDescending(p => p.Date).First();
                var price = decimal.Parse(latestPrice.Price.Split('/')[0]) / decimal.Parse(latestPrice.Price.Split('/')[1]);
                return amount * price;
            }
        }
    }
}