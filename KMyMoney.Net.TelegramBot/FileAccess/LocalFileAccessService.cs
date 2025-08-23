using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.FileAccess;

public class LocalFileAccessService(
    IOptions<LocalStorageSettings> localStorageSettings) : IFileAccessService
{
    public Task<IFileAccessor?> CreateFileAccessorAsync(Message message, CancellationToken cancellationToken)
        => Task.FromResult<IFileAccessor?>(new LocalFileAccessor());

    public Task<string?> GetFilePathAsync(Message message, CancellationToken cancellationToken)
        => Task.FromResult<string?>(localStorageSettings.Value.FilePath);
}