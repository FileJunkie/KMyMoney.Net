using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionCurrencyHandler(
    TelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    AddTransactionPriceHandler addTransactionPriceHandler) : IConditionalStatusHandler
{
    public string HandledStatus => "AddTransactionEnteringCurrency";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
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
            await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
                message.From!.Id,
                UserSettings.Status,
                null,
                cancellationToken: cancellationToken);
            return;
        }

        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
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
        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            addTransactionPriceHandler.HandledStatus,
            cancellationToken: cancellationToken);
    }
}