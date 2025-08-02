using System.Text;
using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands;

public class AccountsCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    TelegramBotClientWrapper botClient) :
    ICommand
{
    public string Command => "accounts";
    public string Description => "Get accounts from .kmy file";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var token = await settingsPersistenceLayer.GetTokenByUserIdAsync(message.From!.Id, cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            await botClient.Bot.SendMessage(
                message.Chat.Id,
                "Use /login + /logincode to set access token",
                cancellationToken: cancellationToken);
            return;
        }

        var filePath = await settingsPersistenceLayer.GetFilePathByUserIdAsync(message.From!.Id, cancellationToken);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            await botClient.Bot.SendMessage(message.Chat.Id, "Use /file to set file path",
                cancellationToken: cancellationToken);
            return;
        }

        await botClient.Bot.SendMessage(message.Chat.Id, $"Wait, let me load the file",
            cancellationToken: cancellationToken);
        var fileAccessor = new DropboxFileAccessor(token);
        var fileUri = new Uri($"dropbox://{filePath}");
        var fileLoader = new KMyMoneyLoaderBuilder()
            .WithFileAccessor(fileAccessor)
            .Build();
        var file = await fileLoader.LoadFileAsync(fileUri);
        await botClient.Bot.SendMessage(message.Chat.Id, $"Loaded", cancellationToken: cancellationToken);

        var sb = new StringBuilder();
        foreach (var (account, i) in file
                     .Root
                     .Accounts
                     .Values
                     .Where(acc => !acc.IsClosed)
                     .Select((x, y) => (x, y)))
        {
            sb.AppendLine($"Id: {account.Id} name: {account.Name}");
            if (i % 10 == 0)
            {
                var answer = sb.ToString();
                await botClient.Bot.SendMessage(message.Chat.Id, answer,
                    cancellationToken: cancellationToken);
                sb.Clear();
            }
        }

        if (sb.Length > 0)
        {
            await botClient.Bot.SendMessage(message.Chat.Id, sb.ToString(),
                cancellationToken: cancellationToken);
        }
    }
}