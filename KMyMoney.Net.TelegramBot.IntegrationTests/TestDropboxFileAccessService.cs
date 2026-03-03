using System.Text.Json;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.IntegrationTests;

internal sealed class TestDropboxFileAccessService(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    TestDropboxFileAccessor fileAccessor,
    bool throwUnexpectedError = false) : IFileAccessService
{
    public async Task<IFileAccessor> CreateFileAccessorAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        if (throwUnexpectedError)
        {
            throw new InvalidOperationException("Unexpected test exception");
        }

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

        return fileAccessor;
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
