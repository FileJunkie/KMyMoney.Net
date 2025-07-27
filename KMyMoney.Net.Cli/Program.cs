using System.CommandLine;
using KMyMoney.Net.Cli.Commands;
using KMyMoney.Net.Cli.Options;

var rootCommand = new RootCommand("KMyMoney.NET CLI tool")
{
    Options =
    {
        BaseOptions.File,
    },
    Subcommands =
    {
        new Command("account")
        {
            Subcommands =
            {
                AccountCommands.Get,
                AccountCommands.List,
            }
        },
        new Command("transaction")
        {
            Subcommands =
            {
                TransactionCommands.Add,
                TransactionCommands.Get,
                TransactionCommands.List,
            }
        },
        DumpCommand.Command,
    }
};

var parseResult = rootCommand.Parse(args);
return await parseResult.InvokeAsync();