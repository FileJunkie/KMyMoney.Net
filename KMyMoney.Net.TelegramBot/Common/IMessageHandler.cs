using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Common;

public interface IMessageHandler
{
    Task HandleAsync(Message message, CancellationToken cancellationToken);
}