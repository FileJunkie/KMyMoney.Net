using System.Text.Json;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Dropbox;

public class DropboxFileAccessService(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    ITelegramBotClientWrapper botClientWrapper) : IFileAccessService
{
    public async Task<IFileAccessor?> CreateFileAccessorAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var token = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Token,
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            await botClientWrapper.Bot.SendMessageAsync(
                message.Chat.Id,
                "Use /login to set access token",
                cancellationToken: cancellationToken);

            await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
                message.From!.Id,
                UserSettings.LastFailedMessage,
                value: JsonSerializer.Serialize(message),
                expiresIn: TimeSpan.FromMinutes(15),
                cancellationToken: cancellationToken);
            return null;
        }

        return new DropboxFileAccessor(token);
    }

    public async Task<string?> GetFilePathAsync(Message message, CancellationToken cancellationToken)
    {
        var filePath = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.FilePath,
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            await botClientWrapper.Bot.SendMessageAsync(
                message.Chat.Id,
                "Use /file to set file path",
                cancellationToken: cancellationToken);
            return null;
        }

        return filePath;
    }
}