using CommandLine;
using KMyMoney.Net.Cli.Commands;
using KMyMoney.Net.Cli.Options;

return Parser.Default.ParseArguments<AccountOptions, TestDumpOptions, AddTransactionOptions>(args)
    .MapResult(
        (AccountOptions opts) => {
            AccountCommands.Execute(opts);
            return 0;
        },
        (TestDumpOptions opts) => {
            TestDumpCommands.Execute(opts);
            return 0;
        },
        (AddTransactionOptions opts) => {
            TransactionCommands.Execute(opts);
            return 0;
        },
        errs => 1);