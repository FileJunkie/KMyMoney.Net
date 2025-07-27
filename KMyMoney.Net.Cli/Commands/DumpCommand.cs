using System.CommandLine;
using KMyMoney.Net.Cli.Options;
using KMyMoney.Net.Core;

namespace KMyMoney.Net.Cli.Commands;

public static class DumpCommand
{
    public static Command Command { get; }

    static DumpCommand()
    {
        Command = new Command("dump");
        Command.SetAction(parseResult =>
        {
            Console.WriteLine(KmyMoneyFileExtensions.Load(parseResult.GetRequiredValue(BaseOptions.File)).Dump());
        });
    }
}