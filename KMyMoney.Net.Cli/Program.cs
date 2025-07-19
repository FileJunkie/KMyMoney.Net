using CommandLine;
using KMyMoney.Net.Cli.Commands;
using KMyMoney.Net.Cli.Options;

public class Program
{
    public static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<AccountOptions, TransactionOptions>(args)
            .MapResult(
                (AccountOptions opts) => AccountCommands.Execute(opts),
                (TransactionOptions opts) => TransactionCommands.Execute(opts),
                errs => 1);
    }
}
