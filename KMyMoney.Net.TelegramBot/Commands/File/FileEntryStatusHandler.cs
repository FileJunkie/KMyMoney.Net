using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.File;

public class FileEntryStatusHandler(
    TelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer) : IConditionalStatusHandler
{
    public string HandledStatus => "EnteringFileName";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
        {
            await botClient.Bot.SendMessage(
                message.Chat.Id,
                "Empty path, really?",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }
        else
        {
            await botClient.Bot.SendMessage(
                message.Chat.Id,
                "Got your file path, saving",
                replyMarkup: new ReplyKeyboardRemove(),
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