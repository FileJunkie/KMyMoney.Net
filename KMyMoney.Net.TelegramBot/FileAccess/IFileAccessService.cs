using KMyMoney.Net.Core.FileAccessors;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.FileAccess;

public interface IFileAccessService
{
    Task<IFileAccessor> CreateFileAccessorAsync(
        Message message,
        CancellationToken cancellationToken);

    Task<string> GetFilePathAsync(
        Message message,
        CancellationToken cancellationToken);
}