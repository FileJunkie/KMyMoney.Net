using CommandLine;

namespace KMyMoney.Net.Cli.Options;

[Verb("test-dump", HelpText = "Read a KMyMoney file, deserialize it, and serialize it back to an XML file.")]
public class TestDumpOptions : BaseOptions
{
    [Option('o', "output", Required = true, HelpText = "The path to the output XML file.")]
    public string? OutputFile { get; set; }
}