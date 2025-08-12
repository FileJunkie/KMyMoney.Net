using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Common;

public abstract class AbstractMessageHandler(
    ISettingsPersistenceLayer settingsPersistenceLayer) : IMessageHandler
{
    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            null,
            cancellationToken: cancellationToken);
        await HandleAfterResettingStatusAsync(message, cancellationToken);
    }

    protected abstract Task HandleAfterResettingStatusAsync(Message message, CancellationToken cancellationToken);
}
