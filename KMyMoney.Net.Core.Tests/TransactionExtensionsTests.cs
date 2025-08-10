using System.Globalization;
using KMyMoney.Net.Models;
using Shouldly;

namespace KMyMoney.Net.Core.Tests;

public class TransactionExtensionsTests
{
    [Fact]
    public void AddTransaction_ShouldAddTransaction()
    {
        // Arrange
        var kmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var fromAccount = new Account { Id = "A000001", Name = "Checking Account", Currency = "USD", Type = "Asset" };
        var toAccount = new Account { Id = "A000002", Name = "Savings Account", Currency = "USD", Type = "Asset" };
        kmyMoneyFileRoot.Accounts.Values = [fromAccount, toAccount];

        // Act
        var transaction = kmyMoneyFileRoot.AddTransaction(
            from: fromAccount.Id,
            to: toAccount.Id,
            amount: 100.0m,
            currency: "USD",
            memo: "Test transaction");

        // Assert
        transaction.ShouldNotBeNull();
        kmyMoneyFileRoot.Transactions.Values.Length.ShouldBe(1);
        transaction.Id.ShouldBe("T000000000000000001");
        transaction.Memo.ShouldBe("Test transaction");
        transaction.Splits.Split.Length.ShouldBe(2);
    }

    [Fact]
    public void AddTransaction_ShouldGenerateCorrectId_WhenTransactionsExist()
    {
        // Arrange
        var kmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var fromAccount = new Account { Id = "A000001", Name = "Checking Account", Currency = "USD", Type = "Asset" };
        var toAccount = new Account { Id = "A000002", Name = "Savings Account", Currency = "USD", Type = "Asset" };
        kmyMoneyFileRoot.Accounts.Values = [fromAccount, toAccount];
        kmyMoneyFileRoot.Transactions.Values =
        [
            new Transaction
            {
                Id = "T000000000000000005",
                PostDate = "2025-08-10",
                EntryDate = "2025-08-10",
                Commodity = "USD",
                Splits = new Splits { Split = [] }
            }
        ];

        // Act
        var transaction = kmyMoneyFileRoot.AddTransaction(
            from: fromAccount.Id,
            to: toAccount.Id,
            amount: 100.0m,
            currency: "USD",
            memo: "Test transaction");

        // Assert
        transaction.Id.ShouldBe("T000000000000000006");
    }

    [Fact]
    public void AddTransaction_ShouldHandleDifferentCurrencies_ToAccount()
    {
        // Arrange
        var kmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var fromAccount = new Account { Id = "A000001", Name = "Checking Account", Currency = "USD", Type = "Asset" };
        var toAccount = new Account { Id = "A000002", Name = "Savings Account", Currency = "EUR", Type = "Asset" };
        kmyMoneyFileRoot.Accounts.Values = [fromAccount, toAccount];
        kmyMoneyFileRoot.Prices.Values =
        [
            new PricePair
            {
                From = "EUR",
                To = "USD",
                Price = [new PriceObj { Date = "2025-08-10", Source = "user", Price = "13/10" }]
            }
        ];

        // Act
        var transaction = kmyMoneyFileRoot.AddTransaction(
            from: fromAccount.Id,
            to: toAccount.Id,
            amount: 130m,
            currency: "USD",
            memo: "Cross-currency transaction");

        // Assert
        transaction.Splits.Split[0].Account.ShouldBe(fromAccount.Id);
        transaction.Splits.Split[0].Value.ShouldBe("-130");
        transaction.Splits.Split[0].Shares.ShouldBe("-130");
        transaction.Splits.Split[1].Account.ShouldBe(toAccount.Id);
        transaction.Splits.Split[1].Value.ShouldBe("130");
        transaction.Splits.Split[1].Shares.ShouldBe("100");
    }

    [Fact]
    public void AddTransaction_ShouldHandleDifferentCurrencies_FromAccount()
    {
        // Arrange
        var kmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var fromAccount = new Account { Id = "A000001", Name = "Checking Account", Currency = "EUR", Type = "Asset" };
        var toAccount = new Account { Id = "A000002", Name = "Savings Account", Currency = "USD", Type = "Asset" };
        kmyMoneyFileRoot.Accounts.Values = [fromAccount, toAccount];
        kmyMoneyFileRoot.Prices.Values =
        [
            new PricePair
            {
                From = "EUR",
                To = "USD",
                Price = [new PriceObj { Date = "2025-08-10", Source = "user", Price = "13/10" }]
            }
        ];

        // Act
        var transaction = kmyMoneyFileRoot.AddTransaction(
            from: fromAccount.Id,
            to: toAccount.Id,
            amount: 130m,
            currency: "USD",
            memo: "Cross-currency transaction");

        // Assert
        transaction.Splits.Split[0].Account.ShouldBe(fromAccount.Id);
        transaction.Splits.Split[0].Value.ShouldBe("-130");
        transaction.Splits.Split[0].Shares.ShouldBe("-100");
        transaction.Splits.Split[1].Account.ShouldBe(toAccount.Id);
        transaction.Splits.Split[1].Value.ShouldBe("130");
        transaction.Splits.Split[1].Shares.ShouldBe("130");
    }

    [Fact]
    public void AddTransaction_ShouldThrowException_WhenFromAccountNotFound()
    {
        // Arrange
        var kmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var toAccount = new Account { Id = "A000002", Name = "Savings Account", Currency = "USD", Type = "Asset" };
        kmyMoneyFileRoot.Accounts.Values = [toAccount];

        // Act & Assert
        var exception = Should.Throw<Exception>(() =>
            kmyMoneyFileRoot.AddTransaction(
                from: "A000001",
                to: toAccount.Id,
                amount: 100.0m,
                currency: "USD",
                memo: "Test transaction"));

        exception.Message.ShouldBe("Source account 'A000001' not found.");
    }

    [Fact]
    public void AddTransaction_ShouldThrowException_WhenToAccountNotFound()
    {
        // Arrange
        var kmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var fromAccount = new Account { Id = "A000001", Name = "Checking Account", Currency = "USD", Type = "Asset" };
        kmyMoneyFileRoot.Accounts.Values = [fromAccount];

        // Act & Assert
        var exception = Should.Throw<Exception>(() =>
            kmyMoneyFileRoot.AddTransaction(
                from: fromAccount.Id,
                to: "A000002",
                amount: 100.0m,
                currency: "USD",
                memo: "Test transaction"));

        exception.Message.ShouldBe("Destination account 'A000002' not found.");
    }

    [Fact]
    public void GetLatestTransactionsByAccountId_ShouldReturnLatestDates()
    {
        // Arrange
        var transactions = new Transactions
        {
            Values =
            [
                new() { Id = "T1", EntryDate = "2025-01-10", Commodity = "USD", PostDate = "2025-01-10", Splits = new() { Split = [new() { Id = "S1", Account = "A1", Value = "1", Shares = "1" }]}},
                new() { Id = "T2", EntryDate = "2025-01-15", Commodity = "USD", PostDate = "2025-01-15", Splits = new() { Split = [new() { Id = "S2", Account = "A1", Value = "1", Shares = "1" }]}},
                new() { Id = "T3", EntryDate = "2025-02-01", Commodity = "USD", PostDate = "2025-02-01", Splits = new() { Split = [new() { Id = "S3", Account = "A2", Value = "1", Shares = "1" }]}},
                new() { Id = "T4", EntryDate = "2025-01-20", Commodity = "USD", PostDate = "2025-01-20", Splits = new() { Split = [new() { Id = "S4", Account = "A1", Value = "1", Shares = "1" }]}},
            ]
        };

        // Act
        var result = transactions.GetLatestTransactionsByAccountId();

        // Assert
        result.Count.ShouldBe(2);
        var expectedDate1 = DateTimeOffset.ParseExact("2025-01-20", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var expectedDate2 = DateTimeOffset.ParseExact("2025-02-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        result["A1"].ShouldBe(expectedDate1);
        result["A2"].ShouldBe(expectedDate2);
    }

    [Fact]
    public void GetLatestTransactionsByAccountId_ShouldReturnEmpty_WhenNoTransactions()
    {
        // Arrange
        var transactions = new Transactions { Values = [] };

        // Act
        var result = transactions.GetLatestTransactionsByAccountId();

        // Assert
        result.ShouldBeEmpty();
    }
}
