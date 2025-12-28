using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Common;

public abstract class ResettableStatusMessageHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer) : IMessageHandler
{
    protected readonly ISettingsPersistenceLayer SettingsPersistenceLayer = settingsPersistenceLayer;

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var status = await SettingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            cancellationToken);

        await SettingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            null,
            cancellationToken: cancellationToken);

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

            if (e.KeepStatus)
            {
                await SettingsPersistenceLayer.SetUserSettingByUserIdAsync(
                    message.From!.Id,
                    UserSettings.Status,
                    status,
                    cancellationToken: cancellationToken);
            }
        }
    }

    protected abstract Task HandleInternalAsync(Message message, CancellationToken cancellationToken);
}
