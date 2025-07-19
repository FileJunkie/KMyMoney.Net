using System;
using KMyMoney.Net.Core;
using KMyMoney.Net.Cli.Options;

namespace KMyMoney.Net.Cli.Commands;

public static class TransactionCommands
{
    public static int Execute(TransactionOptions opts)
    {
        if (opts.FilePath == null) return 1;
        var kmyMoneyFile = KMyMoneyFileLoader.Load(opts.FilePath);
        if (kmyMoneyFile == null)
        {
            Console.WriteLine("Error loading or parsing the file.");
            return 1;
        }
        var repo = new TransactionRepository(kmyMoneyFile);

        if (string.IsNullOrEmpty(opts.Id))
        {
            foreach (var transaction in repo.GetAll())
            {
                Console.WriteLine($"{transaction.Id}: {transaction.PostDate}");
            }
        }
        else
        {
            var transaction = repo.GetById(opts.Id);
            if (transaction != null)
            {
                Console.WriteLine($"{transaction.Id}: {transaction.PostDate}");
            }
            else
            {
                Console.WriteLine("Transaction not found.");
            }
        }
        return 0;
    }
}
