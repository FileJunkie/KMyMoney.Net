using KMyMoney.Net.Core;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.FileAccess;

public interface IFileLoader
{
    Task<KMyMoneyFile?> LoadKMyMoneyFileOrSendErrorAsync(
        Message message,
        CancellationToken cancellationToken);
}