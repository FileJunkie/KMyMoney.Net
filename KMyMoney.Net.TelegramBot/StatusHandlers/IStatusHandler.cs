using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.StatusHandlers;

public interface IStatusHandler
{
    Task HandleAsync(Message message, CancellationToken cancellationToken);
}