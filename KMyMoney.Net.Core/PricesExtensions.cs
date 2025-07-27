using Fractions;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public static class PricesExtensions
{
    public static Fraction ConvertCurrency(
        this Prices prices,
        decimal amount,
        string fromCurrency,
        string toCurrency)
    {
        var pricePair = prices.Values.FirstOrDefault(p => p.From == fromCurrency && p.To == toCurrency);

        Fraction? price = null;
        if (pricePair != null)
        {
            var latestPrice = pricePair.Price.OrderByDescending(p => p.Date).First();
            price = Fraction.FromString(latestPrice.Price);
        }

        pricePair = prices.Values.FirstOrDefault(p => p.From == toCurrency && p.To == fromCurrency);

        if (pricePair != null)
        {
            var latestPrice = pricePair.Price.OrderByDescending(p => p.Date).First();
            price = Fraction.FromString(latestPrice.Price).Reciprocal();
        }

        if (price == null)
        {
            throw new($"No exchange rate found for {fromCurrency} to {toCurrency}");
        }

        return amount * price.Value;
    }
}