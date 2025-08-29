using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public abstract class AbstractAccountSavingHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IConditionalStatusHandler nextStatusHandler,
    IFileLoader fileLoader) :
    AbstractMessageHandlerWithNextStep(settingsPersistenceLayer, nextStatusHandler)
{
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer;

    protected sealed override async Task<bool> HandleInternalAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var file = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(
            message, cancellationToken);
        if (file == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(message.Text) ||
            !file.Root.Accounts.Values.Select(acc => acc.Name)
                .Concat(file.Root.Accounts.Values.Select(acc => acc.Id))
                .Contains(message.Text))
        {
            await botClient
                .Bot
                .SendMessageAsync(
                    message.Chat.Id,
                    "Wrong account, aborting",
                    cancellationToken: cancellationToken);
            return false;
        }

        await _settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            TargetSetting,
            message.Text,
            cancellationToken: cancellationToken);

        await ContinueAfterSavingAccount(file, message, cancellationToken);
        return true;
    }

    protected abstract Task ContinueAfterSavingAccount(
        KMyMoneyFile file,
        Message message,
        CancellationToken cancellationToken);

    protected abstract UserSettings TargetSetting { get; }
}