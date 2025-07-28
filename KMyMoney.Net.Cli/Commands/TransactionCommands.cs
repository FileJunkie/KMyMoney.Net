using System.CommandLine;
using KMyMoney.Net.Cli.Options;
using KMyMoney.Net.Core;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Cli.Commands;

public static class TransactionCommands
{
    public static Command Get { get; }
    public static Command List { get; }
    public static Command Add { get; }

    static TransactionCommands()
    {
        Get = CreateGetCommand();
        List = CreateListCommand();
        Add = CreateAddCommand();
    }
    
    private static Command CreateGetCommand()
    {
        var idOption = new Option<string>("--id")
        {
            Required = false,
        };

        var result = new Command("get")
        {
            idOption
        };
        
        result.SetAction(async parseResult =>
        {
            var id = parseResult.GetValue(idOption);
            var transactions = (await parseResult.GetRequiredValue(BaseOptions.File)).Root.Transactions.Values.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(id))
            {
                transactions = transactions.Where(a => a.Id == id);
            }

            OutputTransactions(transactions.ToArray());
        });
        
        return result;
    }
    
    private static Command CreateListCommand()
    {
        var result = new Command("list");
        result.SetAction(async parseResult =>
        {
            OutputTransactions((await parseResult.GetRequiredValue(BaseOptions.File)).Root.Transactions.Values);
        });
        
        return result;
    }

    private static Command CreateAddCommand()
    {
        var from = new Option<string>("--from")
        {
            Required = true,
        };

        var to = new Option<string>("--to")
        {
            Required = true,
        };

        var amount = new Option<decimal>("--amount")
        {
            Required = true,
        };

        var currency = new Option<string>("--currency")
        {
            Required = true,
        };

        var memo = new Option<string>("--memo")
        {
            Required = false,
        };
        
        var result = new Command("add")
        {
            from, to, amount, currency, memo
        };
        result.SetAction(async parseResult =>
        {
            var file = await parseResult.GetRequiredValue(BaseOptions.File);

            file.Root.AddTransaction(
                parseResult.GetRequiredValue(from),
                parseResult.GetRequiredValue(to),
                parseResult.GetRequiredValue(amount),
                parseResult.GetRequiredValue(currency),
                parseResult.GetValue(memo));
            file.Save(file.Uri);
        });

        return result;
    }

    private static void OutputTransactions(params Transaction[] transactions)
    {
        foreach (var transaction in transactions)
        {
            Console.WriteLine(transaction.Id);
        }
    }

}
