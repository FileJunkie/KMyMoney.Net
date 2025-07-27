using CommandLine;

namespace KMyMoney.Net.Cli.Options;

[Verb("add-transaction", HelpText = "Add a new transaction.")]
public class AddTransactionOptions : BaseOptions
{
    [Option('s', "from", Required = true, HelpText = "Source account name or ID.")]
    public required string From { get; set; }

    [Option('t', "to", Required = true, HelpText = "Destination account name or ID.")]
    public required string To { get; set; }

    [Option('a', "amount", Required = true, HelpText = "Transaction amount.")]
    public decimal Amount { get; set; }

    [Option('c', "currency", Required = true, HelpText = "Transaction currency symbol (e.g. USD).")]
    public required string Currency { get; set; }
        
    [Option('m', "memo", Required = false, HelpText = "Transaction memo.")]
    public string? Memo { get; set; }
}