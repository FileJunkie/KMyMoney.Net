using System.Text.Json;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Dropbox;

public class DropboxFileAccessService(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IDropboxTokenManager dropboxTokenManager) : IFileAccessService
{
    public async Task<IFileAccessor> CreateFileAccessorAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        try
        {
            var accessToken = await dropboxTokenManager.GetValidAccessTokenAsync(
                message.From!.Id,
                cancellationToken);

            return new DropboxFileAccessor(accessToken);
        }
        catch (TokenRefreshFailedException ex)
        {
            await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
                message.From!.Id,
                UserSettings.Token,
                null,
                cancellationToken: cancellationToken);

            throw new WithUserMessageException(
                ex.Message,
                keepStatus: false);
        }
        catch (Exception ex) when (IsTokenRelatedError(ex))
        {
            await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
                message.From!.Id,
                UserSettings.Token,
                null,
                cancellationToken: cancellationToken);

            throw new WithUserMessageException(
                "Dropbox session expired. Please use /login to re-authorize.",
                keepStatus: false);
        }
    }

    private static bool IsTokenRelatedError(Exception ex)
    {
        return (ex.InnerException != null && IsTokenRelatedError(ex.InnerException))
            || ex.Message.Contains("401")
            || ex.Message.Contains("Unauthorized")
            || ex.Message.Contains("invalid_token")
            || ex.Message.Contains("access denied");
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