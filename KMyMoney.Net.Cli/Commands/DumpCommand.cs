using System.CommandLine;
using KMyMoney.Net.Cli.Options;

namespace KMyMoney.Net.Cli.Commands;

public static class DumpCommand
{
    public static Command Command { get; }

    static DumpCommand()
    {
        Command = new Command("dump");
        Command.SetAction(async parseResult =>
        {
            var file = await parseResult.GetRequiredValue(BaseOptions.File);

            Console.WriteLine(file.Dump());
        });
    }
}