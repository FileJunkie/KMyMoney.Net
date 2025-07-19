using System;
using System.IO;
using KMyMoney.Net.Core;
using KMyMoney.Net.Cli.Options;

namespace KMyMoney.Net.Cli.Commands;

public static class AccountCommands
{
    public static void Execute(AccountOptions opts)
    {
        var kmyMoneyFile = KMyMoneyFileLoader.Load(opts.FilePath);
        if (kmyMoneyFile == null)
        {
            throw new InvalidDataException("Error loading or parsing the file. It might be corrupted or not a valid KMyMoney file.");
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
                throw new InvalidOperationException($"Account with ID '{opts.Id}' not found.");
            }
        }
    }
}