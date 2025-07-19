using CommandLine;

namespace KMyMoney.Net.Cli.Options;

public abstract class BaseOptions
{
    [Option('f', "file", Required = true, HelpText = "The KMyMoney file to process.")]
    public required string FilePath { get; init; }
}
