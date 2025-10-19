using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Common;

public abstract class AbstractMessageHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer) : IMessageHandler
{
    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var status = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            cancellationToken);

        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            null,
            cancellationToken: cancellationToken);

        try
        {
            await HandleAfterResettingStatusAsync(message, cancellationToken);
        }
        catch (WithUserMessageException e)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                e.Message,
                cancellationToken: cancellationToken);

            if (e.KeepStatus)
            {
                await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
                    message.From!.Id,
                    UserSettings.Status,
                    status,
                    cancellationToken: cancellationToken);
            }
        }
    }

    protected abstract Task HandleAfterResettingStatusAsync(Message message, CancellationToken cancellationToken);
}
