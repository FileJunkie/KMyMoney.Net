using KMyMoney.Net.TelegramBot.Common;
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
    AddTransactionPriceHandler addTransactionPriceHandler) :
    AbstractMessageHandlerWithNextStep(settingsPersistenceLayer, addTransactionPriceHandler), IConditionalStatusHandler
{
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer;
    public string HandledStatus => "AddTransactionEnteringCurrency";

    protected override async Task HandleInternalAsync(Message message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message.Text))
        {
            await botClient
                .Bot
                .SendMessage(
                    message.Chat.Id,
                    "Currency no chosen, aborting",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            return;
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
    }
}