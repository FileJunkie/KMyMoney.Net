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
        kmyMoneyFileRoot.Transactions.Values.ShouldHaveSingleItem();
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
    public void AddTransaction_ShouldHandleDifferentCurrencies()
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
                Price =
                [
                    new PriceObj
                    {
                        Date = "2025-08-10",
                        Source = "user",
                        Price = "13/10"
                    }
                ]
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
}
