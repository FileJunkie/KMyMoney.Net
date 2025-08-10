using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    TelegramBotClientWrapper botClient,
    AddTransactionFromAccountHandler addTransactionFromAccountHandler)
    : ICommand
{
    public string Command => "add_transaction";
    public string Description => "Adds a new transaction";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var file = await FileLoaderHelpers.LoadKMyMoneyFileOrSendErrorAsync(
            settingsPersistenceLayer, botClient.Bot, message, cancellationToken);
        if (file == null)
        {
            return;
        }

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
                "Choose account to take money from",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            addTransactionFromAccountHandler.HandledStatus,
            cancellationToken: cancellationToken);
    }
}