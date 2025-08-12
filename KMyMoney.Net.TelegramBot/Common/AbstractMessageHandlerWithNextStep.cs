using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Common;

public abstract class AbstractMessageHandlerWithNextStep(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IConditionalStatusHandler nextStatusHandler) : AbstractMessageHandler(settingsPersistenceLayer)
{
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer;

    protected override async Task HandleAfterResettingStatusAsync(Message message, CancellationToken cancellationToken)
    {
        await HandleInternalAsync(message, cancellationToken);
        await _settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            nextStatusHandler.HandledStatus,
            cancellationToken: cancellationToken);
    }

    protected abstract Task HandleInternalAsync(Message message, CancellationToken cancellationToken);
}