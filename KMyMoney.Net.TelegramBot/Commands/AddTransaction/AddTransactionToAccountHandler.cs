using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionToAccountHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    AddTransactionCurrencyHandler addTransactionCurrencyHandler,
    IFileLoader fileLoader) :
    AbstractAccountSavingHandler(botClient, settingsPersistenceLayer, addTransactionCurrencyHandler, fileLoader), IConditionalStatusHandler
{
    private readonly ITelegramBotClientWrapper _botClient = botClient;
    public string HandledStatus => "AddTransactionEnteringToAccount";
    protected override UserSettings TargetSetting => UserSettings.AccountTo;

    protected override async Task ContinueAfterSavingAccount(
        KMyMoneyFile file,
        Message message,
        CancellationToken cancellationToken)
    {
        var currencies = file.Root.Prices.Values.Select(v => v.From)
            .Concat(file.Root.Prices.Values.Select(v => v.To))
            .Where(c => c.Length == 3)
            .Distinct();

        var keyboard = currencies.SplitBy(5);

        await _botClient
            .Bot
            .SendMessage(
                message.Chat.Id,
                "Choose currency",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
    }
}