using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionToAccountHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    AddTransactionCurrencyHandler addTransactionCurrencyHandler) : IConditionalStatusHandler
{
    public string HandledStatus => "AddTransactionEnteringToAccount";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var file = await FileLoaderHelpers.LoadKMyMoneyFileOrSendErrorAsync(
            settingsPersistenceLayer, botClient.Bot, message, cancellationToken);
        if (file == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(message.Text) ||
            !file.Root.Accounts.Values.Select(acc => acc.Name)
                .Concat(file.Root.Accounts.Values.Select(acc => acc.Id))
                .Contains(message.Text))
        {
            await botClient
                .Bot
                .SendMessage(
                    message.Chat.Id,
                    "Wrong account, aborting",
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
            UserSettings.AccountTo,
            message.Text,
            cancellationToken: cancellationToken);
        
        var currencies = file.Root.Prices.Values.Select(v => v.From)
            .Concat(file.Root.Prices.Values.Select(v => v.To))
            .Where(c => c.Length == 3)
            .Distinct();

        var keyboard = currencies.SplitBy(5);

        await botClient
            .Bot
            .SendMessage(
                message.Chat.Id,
                "Choose currency",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            addTransactionCurrencyHandler.HandledStatus,
            cancellationToken: cancellationToken);
    }
}