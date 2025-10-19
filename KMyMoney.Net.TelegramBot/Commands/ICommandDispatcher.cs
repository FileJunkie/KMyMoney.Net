using KMyMoney.Net.TelegramBot.StatusHandlers;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands;

public interface ICommandDispatcher : IStatusHandler
{
    bool MessageContainsCommand(Message message);
}