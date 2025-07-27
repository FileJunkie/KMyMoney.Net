using System.CommandLine;
using KMyMoney.Net.Core;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Cli.Options;

public static class BaseOptions
{
    public static Option<string> File { get; } = new ("--file", "-f")
    {
        Description = "Input KMyMoney file",
        Recursive = true,
        Required = true,
    };
}