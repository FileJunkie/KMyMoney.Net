using System.CommandLine;
using KMyMoney.Net.Cli.Options;
using KMyMoney.Net.Core;
using KMyMoney.Net.Dropbox;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Cli.Commands;

public static class DumpCommand
{
    public static Command Command { get; }

    static DumpCommand()
    {
        Command = new Command("dump");
        Command.SetAction(async parseResult =>
        {
            var filePath = parseResult.GetRequiredValue(BaseOptions.File);
            KmyMoneyFile file;
            if (filePath.StartsWith("dropbox://"))
            {
                var dropboxFileAccessor = await DropboxFileAccessor.CreateAsync(
                    apiKey: Environment.GetEnvironmentVariable("DROPBOX_API_KEY")!,
                    apiSecret: Environment.GetEnvironmentVariable("DROPBOX_API_SECRET")!,
                    uri =>
                    {
                        Console.WriteLine("Please go here: {0}", uri);
                        Console.Write("Code: ");
                        return Task.FromResult(Console.ReadLine()!);
                    });

                using var fileResponse =
                    await dropboxFileAccessor.GetFileAsync(filePath.Replace("dropbox:/", string.Empty));
                file = KmyMoneyFileExtensions.Load(await fileResponse.GetContentAsStreamAsync());
            }
            else
            {
                file = KmyMoneyFileExtensions.Load(filePath);
            }

            Console.WriteLine(file.Dump());
        });
    }
}