using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionFromAccountHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    AddTransactionToAccountHandler addTransactionToAccountHandler) : IConditionalStatusHandler
{
    public string HandledStatus => "AddTransactionEnteringFromAccount";

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
            UserSettings.AccountFrom,
            message.Text,
            cancellationToken: cancellationToken);

        var lastTransactionPerAccount = file
            .Root
            .Transactions
            .GetLatestTransactionsByAccountId();

        var accounts = file.Root.Accounts.Values
            .Where(acc => !acc.IsClosed)
            .OrderByDescending(acc =>
                lastTransactionPerAccount.TryGetValue(acc.Id, out var lastTransaction) ?
                    lastTransaction : DateTimeOffset.MinValue)
            .Select(acc => acc.Name);

        var keyboard = accounts.SplitBy(3);

        await botClient
            .Bot
            .SendMessage(
                message.Chat.Id,
                "Choose account to put money into",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            addTransactionToAccountHandler.HandledStatus,
            cancellationToken: cancellationToken);
    }
}