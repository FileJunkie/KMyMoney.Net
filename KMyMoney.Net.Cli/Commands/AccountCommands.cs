using System;
using KMyMoney.Net.Core;
using KMyMoney.Net.Cli.Options;

namespace KMyMoney.Net.Cli.Commands;

public static class AccountCommands
{
    public static int Execute(AccountOptions opts)
    {
        if (opts.FilePath == null) return 1;
        var kmyMoneyFile = KMyMoneyFileLoader.Load(opts.FilePath);
        if (kmyMoneyFile == null)
        {
            Console.WriteLine("Error loading or parsing the file.");
            return 1;
        }
        var repo = new AccountRepository(kmyMoneyFile);

        if (string.IsNullOrEmpty(opts.Id))
        {
            foreach (var account in repo.GetAll())
            {
                Console.WriteLine($"{account.Id}: {account.Name}");
            }
        }
        else
        {
            var account = repo.GetById(opts.Id);
            if (account != null)
            {
                Console.WriteLine($"{account.Id}: {account.Name}");
            }
            else
            {
                Console.WriteLine("Account not found.");
            }
        }
        return 0;
    }
}
