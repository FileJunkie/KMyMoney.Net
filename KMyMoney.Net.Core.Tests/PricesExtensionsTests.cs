using Fractions;
using KMyMoney.Net.Models;
using Shouldly;

namespace KMyMoney.Net.Core.Tests;

public class PricesExtensionsTests
{
    [Fact]
    public void ConvertCurrency_ShouldUseDirectRate()
    {
        // Arrange
        var prices = new Prices
        {
            Values =
            [
                new()
                {
                    From = "EUR", To = "USD", Price =
                    [
                        new() { Date = "2025-01-01", Source = "user", Price = "13/10" }
                    ]
                }
            ]
        };

        // Act
        var result = prices.ConvertCurrency(100m, "EUR", "USD");

        // Assert
        result.ShouldBe(Fraction.FromDecimal(130m));
    }

    [Fact]
    public void ConvertCurrency_ShouldUseReciprocalRate()
    {
        // Arrange
        var prices = new Prices
        {
            Values =
            [
                new()
                {
                    From = "USD", To = "EUR", Price =
                    [
                        new() { Date = "2025-01-01", Source = "user", Price = "10/13" }
                    ]
                }
            ]
        };

        // Act
        var result = prices.ConvertCurrency(100m, "EUR", "USD");

        // Assert
        result.ToDecimal().ShouldBe(130m, 0.001m);
    }

    [Fact]
    public void ConvertCurrency_ShouldUseLatestPrice()
    {
        // Arrange
        var prices = new Prices
        {
            Values =
            [
                new()
                {
                    From = "EUR", To = "USD", Price =
                    [
                        new() { Date = "2025-01-01", Source = "user", Price = "12/10" },
                        new() { Date = "2025-01-05", Source = "user", Price = "13/10" },
                        new() { Date = "2025-01-03", Source = "user", Price = "125/100" }
                    ]
                }
            ]
        };

        // Act
        var result = prices.ConvertCurrency(100m, "EUR", "USD");

        // Assert
        result.ShouldBe(Fraction.FromDecimal(130m));
    }

    [Fact]
    public void ConvertCurrency_ShouldThrowWhenNoRateExists()
    {
        // Arrange
        var prices = new Prices { Values = [] };

        // Act & Assert
        var exception = Should.Throw<Exception>(() => prices.ConvertCurrency(100m, "EUR", "USD"));
        exception.Message.ShouldBe("No exchange rate found for EUR to USD");
    }
}
