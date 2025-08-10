using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands;

public abstract class AbstractCommandWithStatus(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IConditionalStatusHandler nextStatusHandler) : ICommand
{
    public abstract string Command { get; }
    public abstract string Description { get; }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        await HandleInternalAsync(message, cancellationToken);
        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            nextStatusHandler.HandledStatus,
            cancellationToken: cancellationToken);
    }

    protected abstract Task HandleInternalAsync(Message message, CancellationToken cancellationToken);
}