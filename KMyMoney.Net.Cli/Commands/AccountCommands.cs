using System.CommandLine;
using KMyMoney.Net.Cli.Options;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Cli.Commands;

public static class AccountCommands
{
    public static Command Get { get; }
    public static Command List { get; }

    static AccountCommands()
    {
        Get = CreateGetCommand();
        List = CreateListCommand();
    }

    private static Command CreateGetCommand()
    {
        var idOption = new Option<string>("--id")
        {
            Required = false,
        };
        var nameOption = new Option<string>("--name")
        {
            Required = false,
        };

        var result = new Command("get")
        {
            idOption, nameOption
        };

        result.SetAction(async parseResult =>
        {
            var id = parseResult.GetValue(idOption);
            var name = parseResult.GetValue(nameOption);
            var accounts = (await parseResult.GetRequiredValue(BaseOptions.File)).Root.Accounts.Values.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(id))
            {
                accounts = accounts.Where(a => a.Id == id);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                accounts = accounts.Where(a => a.Name == name);
            }

            OutputAccounts(accounts.ToArray());
        });

        return result;
    }

    private static Command CreateListCommand()
    {
        var result = new Command("list");
        result.SetAction(async parseResult =>
        {
            OutputAccounts((await parseResult.GetRequiredValue(BaseOptions.File)).Root.Accounts.Values);
        });

        return result;
    }

    private static void OutputAccounts(params Account[] accounts)
    {
        foreach (var account in accounts)
        {
            Console.WriteLine($"{account.Id}: {account.Name}");
        }
    }
}