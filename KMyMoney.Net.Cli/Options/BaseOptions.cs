using System.CommandLine;
using KMyMoney.Net.Core;
using KMyMoney.Net.Dropbox;

namespace KMyMoney.Net.Cli.Options;

public static class BaseOptions
{
    public static Option<Task<KMyMoneyFile>> File { get; } = new ("--file", "-f")
    {
        Description = "Input KMyMoney file",
        Recursive = true,
        Required = true,
        CustomParser = async result =>
        {
            if (result.Tokens.Count != 1)
            {
                result.AddError("--file requires two arguments");
                return null!;
            }

            var dropboxFileAccessor = await DropboxFileAccessor.CreateAsync(
                apiKey: Environment.GetEnvironmentVariable("DROPBOX_API_KEY")!,
                apiSecret: Environment.GetEnvironmentVariable("DROPBOX_API_SECRET")!,
                uri =>
                {
                    Console.WriteLine("Please go here: {0}", uri);
                    Console.Write("Code: ");
                    return Task.FromResult(Console.ReadLine()!);
                });

            var loader = new KMyMoneyLoaderBuilder()
                .WithFileAccessor(dropboxFileAccessor)
                .Build();
            return await loader.LoadFileAsync(new Uri(result.Tokens.Single().Value));
        },
    };
}