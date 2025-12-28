using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Common;

public abstract class ResettableStatusMessageHandlerWithNextStep<TNextStatusHandler>(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer)
    : ResettableStatusMessageHandler(botClient, settingsPersistenceLayer)
    where TNextStatusHandler : IConditionalStatusHandler
{
    protected override async Task HandleInternalAsync(Message message, CancellationToken cancellationToken)
    {
        await HandleAndSetNextStepAsync(message, cancellationToken);
        await SettingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            TNextStatusHandler.HandledStatus,
            cancellationToken: cancellationToken);
    }

    protected abstract Task HandleAndSetNextStepAsync(Message message, CancellationToken cancellationToken);
}
