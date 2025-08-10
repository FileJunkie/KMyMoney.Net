using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Utils;

public static class FileLoaderHelpers
{
    public static async Task<KMyMoneyFile?> LoadKMyMoneyFileOrSendErrorAsync(
        ISettingsPersistenceLayer settingsPersistenceLayer,
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cancellationToken)
    {
        var token = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(message.From!.Id, UserSettings.Token, cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            await botClient.SendMessage(
                message.Chat.Id,
                "Use /login to set access token",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            return null;
        }

        var filePath = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(message.From!.Id, UserSettings.FilePath, cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            await botClient.SendMessage(message.Chat.Id, "Use /file to set file path",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            return null;
        }

        var fileAccessor = new DropboxFileAccessor(token);
        var fileUri = new Uri($"dropbox://{filePath}");
        var fileLoader = new KMyMoneyLoaderBuilder()
            .WithFileAccessor(fileAccessor)
            .Build();
        var file = await fileLoader.LoadFileAsync(fileUri);
        return file;
    }
}