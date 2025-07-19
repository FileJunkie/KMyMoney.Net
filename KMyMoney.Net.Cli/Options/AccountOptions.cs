using CommandLine;

namespace KMyMoney.Net.Cli.Options;

[Verb("accounts", HelpText = "List all accounts or find a specific account by ID.")]
public class AccountOptions : BaseOptions
{
    [Option("id", Required = false, HelpText = "The ID of the account to find. If not provided, all accounts will be listed.")]
    public string? Id { get; init; }
}