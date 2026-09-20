using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Dropbox;

public class DropboxFileAccessService(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IOptions<DropboxSettings> dropboxSettings) : IFileAccessService
{
    public Task<IFileAccessor> CreateFileAccessorAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        return CreateFileAccessorInternalAsync(message, cancellationToken);
    }

    private async Task<IFileAccessor> CreateFileAccessorInternalAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var refreshToken = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.RefreshToken,
            cancellationToken);

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new WithUserMessageException(
                "No Dropbox access. Please use /login to authorize.",
                keepStatus: false);
        }

        return new DropboxFileAccessor(
            refreshToken,
            dropboxSettings.Value);
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
