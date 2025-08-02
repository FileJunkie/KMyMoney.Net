using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands;

public class FileCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    TelegramBotClientWrapper botClient) :
    ICommand
{
    public string Command => "file";
    public string Description => "Setting path do the file inside Dropbox";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var cleanedMessage = message.Text?.Replace($"/{Command}", string.Empty).TrimStart() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(cleanedMessage))
        {
            await botClient.Bot.SendMessage(
                message.Chat.Id,
                "Empty path, really?",
                cancellationToken: cancellationToken);
        }
        else
        {
            await botClient.Bot.SendMessage(
                message.Chat.Id,
                "Got your file path, saving",
                cancellationToken: cancellationToken);
            var path = cleanedMessage.StartsWith('/') ? cleanedMessage : $"/{cleanedMessage}";
            await settingsPersistenceLayer.SetFilePathByUserIdAsync(message.From!.Id, path, cancellationToken);
        }
    }
}