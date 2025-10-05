using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.File;

public class FileEntryStatusHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer) : IConditionalStatusHandler
{
    public static string HandledStatus => "EnteringFileName";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "Empty path, really?",
                cancellationToken: cancellationToken);
        }
        else
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "Got your file path, saving",
                cancellationToken: cancellationToken);
            var path = message.Text.StartsWith('/') ? message.Text : $"/{message.Text}";
            await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
                message.From!.Id,
                UserSettings.FilePath,
                path,
                cancellationToken: cancellationToken);
        }

        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            null,
            cancellationToken: cancellationToken);
    }
}