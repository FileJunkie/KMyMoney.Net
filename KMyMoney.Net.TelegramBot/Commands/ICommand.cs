using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands;

public interface ICommand
{
    string Command { get; }
    string Description { get; }

    Task HandleAsync(Message message, CancellationToken cancellationToken);
}