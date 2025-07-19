using CommandLine;

namespace KMyMoney.Net.Cli.Options;

[Verb("transactions", HelpText = "List all transactions or find a specific transaction by ID.")]
public class TransactionOptions : BaseOptions
{
    [Option("id", Required = false, HelpText = "The ID of the transaction to find. If not provided, all transactions will be listed.")]
    public string? Id { get; set; }
}
