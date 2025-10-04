using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionPriceHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    AddTransactionFromAccountHandler addTransactionFromAccountHandler,
    IFileLoader fileLoader) :
    AbstractMessageHandlerWithNextStep(settingsPersistenceLayer, addTransactionFromAccountHandler),
    IConditionalStatusHandler
{
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer;
    public string HandledStatus => "AddTransactionEnteringPrice";

    protected override async Task<bool> HandleInternalAsync(Message message, CancellationToken cancellationToken)
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
            return false;
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
            return false;
        }

        var file = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(
            message, cancellationToken);
        if (file == null)
        {
            return false;
        }

        var (amount, currency) = ExtractValueAndCurrency(message.Text);
        if (!string.IsNullOrEmpty(currency) &&
            !file.Root.Prices.Values.Any(p => p.From == currency || p.To == currency))
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "What currency is that?",
                cancellationToken: cancellationToken);
            return false;
        }

        currency ??= file.Root
            .Accounts.GetByNameOrId(accountFrom)?
            .Currency;

        if (currency == null)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                $"Problem with finding currency of accountFrom {accountFrom}",
                cancellationToken: cancellationToken);
            return false;
        }

        if (!amount.HasValue)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "What kind of amount is that?",
                cancellationToken: cancellationToken);
            return false;
        }

        file.Root.AddTransaction(
            accountFrom,
            accountTo,
            amount.Value,
            currency,
            null);
        await file.SaveAsync();

        var accounts = file
            .GetAccountsLatestTransactionDescending()
            .Select(acc => acc.Name);

        var keyboard = accounts.SplitBy(3);

        await botClient.Bot.SendMessageAsync(
            message.Chat.Id,
            "Saved.",
            replyMarkup: keyboard,
            disableNotification: true,
            cancellationToken: cancellationToken);

        return true;
    }

    private static (decimal? amount, string? currency) ExtractValueAndCurrency(
        string? messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText))
        {
            return (null, null);
        }

        var components = messageText.Split(" ");
        decimal? amount = null;
        string? currency = null;

        if (decimal.TryParse(components[0], out var val))
        {
            amount = val;
        }

        if (components.Length > 1)
        {
            currency = components[1];
        }

        return (amount, currency);
    }
}
