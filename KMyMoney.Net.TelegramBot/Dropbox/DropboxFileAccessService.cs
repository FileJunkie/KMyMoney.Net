using System.Text.Json;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Dropbox;

public class DropboxFileAccessService(
    ISettingsPersistenceLayer settingsPersistenceLayer) : IFileAccessService
{
    public async Task<IFileAccessor> CreateFileAccessorAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var token = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Token,
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
                message.From!.Id,
                UserSettings.LastFailedMessage,
                value: serializedMessage,
                expiresIn: TimeSpan.FromMinutes(15),
                cancellationToken: cancellationToken);
            throw new WithUserMessageException(
                "Use /login to set access token",
                keepStatus: true);
        }

        return new DropboxFileAccessor(token);
    }

    public async Task<string> GetFilePathAsync(Message message, CancellationToken cancellationToken)
    {
        var filePath = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.FilePath,
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new WithUserMessageException("Use /file to set file path");
        }

        return filePath;
    }
}