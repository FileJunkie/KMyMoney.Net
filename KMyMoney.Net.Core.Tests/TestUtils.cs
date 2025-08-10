using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core.Tests;

public static class TestUtils
{
    public static KmyMoneyFileRoot CreateTestKmyMoneyFileRoot() => new()
    {
        FileInfo = new(),
        User = new()
        {
            Name = "test-user",
            Email = "test@email.com"
        },
        Accounts = new()
        {
            Values =
            [
                new()
                {
                    Id = "A000001",
                    Name = "Checking Account",
                    Type = "Asset",
                    Currency = "USD"
                }
            ]
        },
        Institutions = new() { Values = [] },
        Payees = new() { Values = [] },
        CostCenters = new(),
        Tags = new(),
        Transactions = new() { Values = [] },
        KeyValuePairs = new() { Pair = [] },
        Schedules = new() { Values = [] },
        Securities = new() { Values = [] },
        Currencies = new() { Values = [] },
        Prices = new() { Values = [] },
        Reports = new() { Values = [] },
        Budgets = new(),
        OnlineJobs = new()
    };
}