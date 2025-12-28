using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Common;

public abstract class NonResettableStatusMessageHandler(
    ITelegramBotClientWrapper botClient) : IMessageHandler
{
    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        try
        {
            await HandleInternalAsync(message, cancellationToken);
        }
        catch (WithUserMessageException e)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                e.Message,
                cancellationToken: cancellationToken);
        }
    }

    protected abstract Task HandleInternalAsync(Message message, CancellationToken cancellationToken);
}
