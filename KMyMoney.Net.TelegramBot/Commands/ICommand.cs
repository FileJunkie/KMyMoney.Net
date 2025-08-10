using KMyMoney.Net.TelegramBot.Common;

namespace KMyMoney.Net.TelegramBot.Commands;

public interface ICommand : IMessageHandler
{
    string Command { get; }
    string Description { get; }
}