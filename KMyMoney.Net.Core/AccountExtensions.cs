using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public static class AccountExtensions
{
    public static IOrderedEnumerable<Account> GetAccountsLatestTransactionDescending(
        this KMyMoneyFile file)
    {
        var lastTransactionPerAccount = file
            .Root
            .Transactions
            .GetLatestTransactionsByAccountId();

        return file.Root.Accounts.Values
            .Where(acc => !acc.IsClosed)
            .OrderByDescending(acc =>
                lastTransactionPerAccount.TryGetValue(acc.Id, out var lastTransaction)
                    ? lastTransaction
                    : DateTimeOffset.MinValue);
    }
}