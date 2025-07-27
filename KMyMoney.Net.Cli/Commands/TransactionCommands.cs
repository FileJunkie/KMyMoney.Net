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
        
        result.SetAction(parseResult =>
        {
            var id = parseResult.GetValue(idOption);
            var transactions = KmyMoneyFileExtensions.Load(parseResult.GetRequiredValue(BaseOptions.File)).Transactions.Values.AsEnumerable();
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
        result.SetAction(parseResult =>
        {
            OutputTransactions(KmyMoneyFileExtensions.Load(parseResult.GetRequiredValue(BaseOptions.File)).Transactions.Values);
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
        result.SetAction(parseResult =>
        {
            var filePath = parseResult.GetRequiredValue(BaseOptions.File);
            var file = KmyMoneyFileExtensions.Load(filePath);
            file.AddTransaction(
                parseResult.GetRequiredValue(from),
                parseResult.GetRequiredValue(to),
                parseResult.GetRequiredValue(amount),
                parseResult.GetRequiredValue(currency),
                parseResult.GetValue(memo));
            file.Save(filePath);
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
