using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionFromAccountHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    AddTransactionToAccountHandler addTransactionToAccountHandler,
    IFileLoader fileLoader) :
    AbstractAccountSavingHandler(botClient, settingsPersistenceLayer, addTransactionToAccountHandler, fileLoader), IConditionalStatusHandler
{
    private readonly ITelegramBotClientWrapper _botClient = botClient;
    public string HandledStatus => "AddTransactionEnteringFromAccount";
    protected override UserSettings TargetSetting => UserSettings.AccountFrom;

    protected override async Task ContinueAfterSavingAccount(
        KMyMoneyFile file,
        Message message,
        CancellationToken cancellationToken)
    {
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

        await _botClient
            .Bot
            .SendMessage(
                message.Chat.Id,
                "Choose account to put money into",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
    }
}