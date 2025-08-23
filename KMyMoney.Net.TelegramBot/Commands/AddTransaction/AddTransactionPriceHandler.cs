using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionPriceHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IFileLoader fileLoader) : AbstractMessageHandler(settingsPersistenceLayer), IConditionalStatusHandler
{
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer;
    public string HandledStatus => "AddTransactionEnteringPrice";

    protected override async Task HandleAfterResettingStatusAsync(Message message, CancellationToken cancellationToken)
    {
        var accountFrom = await _settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.AccountFrom,
            cancellationToken: cancellationToken);

        if (accountFrom == null)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "AccountFrom was somehow null?",
                cancellationToken: cancellationToken);
            return;
        }

        var accountTo = await _settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.AccountTo,
            cancellationToken: cancellationToken);

        if (accountTo == null)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "AccountTo was somehow null?",
                cancellationToken: cancellationToken);
            return;
        }

        var currency = await _settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Currency,
            cancellationToken: cancellationToken);

        if (currency == null)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "Currency was somehow null?",
                cancellationToken: cancellationToken);
            return;
        }

        if (!decimal.TryParse(message.Text, out var amount))
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "What kind of amount is that?",
                cancellationToken: cancellationToken);
            return;
        }

        var file = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(
            message, cancellationToken);
        if (file == null)
        {
            return;
        }

        file.Root.AddTransaction(
            accountFrom,
            accountTo,
            amount,
            currency,
            null);
        await file.SaveAsync();

        await botClient.Bot.SendMessageAsync(
            message.Chat.Id,
            "Saved.",
            cancellationToken: cancellationToken);
    }
}
