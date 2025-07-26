using System;
using CommandLine;
using KMyMoney.Net.Cli.Commands;
using KMyMoney.Net.Cli.Options;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            return Parser.Default.ParseArguments<AccountOptions, TransactionOptions, TestDumpOptions>(args)
                .MapResult(
                    (AccountOptions opts) => {
                        AccountCommands.Execute(opts);
                        return 0;
                    },
                    (TransactionOptions opts) => {
                        TransactionCommands.Execute(opts);
                        return 0;
                    },
                    (TestDumpOptions opts) => {
                        TestDumpCommands.Execute(opts);
                        return 0;
                    },
                    errs => 1);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}