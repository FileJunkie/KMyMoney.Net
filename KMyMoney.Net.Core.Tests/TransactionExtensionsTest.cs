using KMyMoney.Net.Models;
using Shouldly;

namespace KMyMoney.Net.Core.Tests;

public class TransactionExtensionsTest
{
    [Fact]
    public void AddTransaction_ShouldAddTransaction()
    {
        // Arrange
        var kmyMoneyFileRoot = CreateMinimalKmyMoneyFileRoot();
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
        var kmyMoneyFileRoot = CreateMinimalKmyMoneyFileRoot();
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
    public void AddTransaction_ShouldThrowException_WhenFromAccountNotFound()
    {
        // Arrange
        var kmyMoneyFileRoot = CreateMinimalKmyMoneyFileRoot();
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
        var kmyMoneyFileRoot = CreateMinimalKmyMoneyFileRoot();
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

    private static KmyMoneyFileRoot CreateMinimalKmyMoneyFileRoot()
    {
        return new KmyMoneyFileRoot
        {
            FileInfo = new(),
            User = new()
            {
                Name = "name",
                Email = "email"
            },
            Institutions = new() { Values = [], },
            Payees = new() { Values = [], },
            CostCenters = new(),
            Tags = new(),
            Accounts = new() { Values = [], },
            Transactions = new() { Values = [], },
            KeyValuePairs = new() { Pair = [], },
            Schedules = new() { Values = [], },
            Securities = new() { Values = [], },
            Currencies = new() { Values = [], },
            Prices = new() { Values = [], },
            Reports = new() { Values = [], },
            Budgets = new(),
            OnlineJobs = new()
        };
    }
}
