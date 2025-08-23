using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionCurrencyHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    AddTransactionPriceHandler addTransactionPriceHandler,
    IFileLoader fileLoader) :
    AbstractMessageHandlerWithNextStep(settingsPersistenceLayer, addTransactionPriceHandler), IConditionalStatusHandler
{
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer;
    public string HandledStatus => "AddTransactionEnteringCurrency";

    protected override async Task<bool> HandleInternalAsync(Message message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
        {
            await botClient
                .Bot
                .SendMessage(
                    message.Chat.Id,
                    "Currency not chosen, aborting",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            return false;
        }

        var file = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(
            message, cancellationToken);
        if (file == null)
        {
            return false;
        }

        var currencies = file.Root.Prices.Values.Select(v => v.From)
            .Concat(file.Root.Prices.Values.Select(v => v.To))
            .Distinct();

        if (!currencies.Contains(message.Text))
        {
            await botClient
                .Bot
                .SendMessage(
                    message.Chat.Id,
                    "Wrong currency",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            return false;
        }

        await _settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Currency,
            message.Text,
            cancellationToken: cancellationToken);

        await botClient
            .Bot
            .SendMessage(
                message.Chat.Id,
                "Enter transaction amount",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        return true;
    }
}