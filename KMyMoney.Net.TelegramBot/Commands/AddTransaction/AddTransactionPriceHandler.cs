using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionPriceHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IFileLoader fileLoader) : IConditionalStatusHandler
{
    public string HandledStatus => "AddTransactionEnteringPrice";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            null,
            cancellationToken: cancellationToken);

        var accountFrom = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
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
        
        var accountTo = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
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

        var currency = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
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
