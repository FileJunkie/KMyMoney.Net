using System;
using System.IO;
using KMyMoney.Net.Core;
using KMyMoney.Net.Cli.Options;

namespace KMyMoney.Net.Cli.Commands;

public static class TransactionCommands
{
    public static void Execute(TransactionOptions opts)
    {
        var kmyMoneyFile = KMyMoneyFileLoader.Load(opts.FilePath);
        if (kmyMoneyFile == null)
        {
            throw new InvalidDataException("Error loading or parsing the file. It might be corrupted or not a valid KMyMoney file.");
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
                throw new InvalidOperationException($"Transaction with ID '{opts.Id}' not found.");
            }
        }
    }
}