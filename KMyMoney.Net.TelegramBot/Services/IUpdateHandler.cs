using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KMyMoney.Net.TelegramBot.Services;

public interface IUpdateHandler
{
    Task OnMessageAsync(Message message, UpdateType type, CancellationToken cancellationToken);
    Task OnErrorAsync(Exception exception, HandleErrorSource source);
}