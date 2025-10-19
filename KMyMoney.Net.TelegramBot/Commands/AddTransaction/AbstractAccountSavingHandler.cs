using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public abstract class AbstractAccountSavingHandler<TNextStatusHandler>(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IFileLoader fileLoader) :
    AbstractMessageHandlerWithNextStep<TNextStatusHandler>(botClient, settingsPersistenceLayer)
    where TNextStatusHandler : IConditionalStatusHandler
{
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer;
    private readonly ITelegramBotClientWrapper _botClient = botClient;

    protected sealed override async Task HandleInternalAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var file = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(
            message, cancellationToken);

        if (string.IsNullOrWhiteSpace(message.Text) ||
            !file.Root.Accounts.Values.Select(acc => acc.Name)
                .Concat(file.Root.Accounts.Values.Select(acc => acc.Id))
                .Contains(message.Text))
        {
            throw new WithUserMessageException("Wrong account, aborting");
        }

        await _settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            TargetSetting,
            message.Text,
            cancellationToken: cancellationToken);

        await ContinueAfterSavingAccount(file, message, cancellationToken);
    }

    protected abstract Task ContinueAfterSavingAccount(
        KMyMoneyFile file,
        Message message,
        CancellationToken cancellationToken);

    protected abstract UserSettings TargetSetting { get; }
}