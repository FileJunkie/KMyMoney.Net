using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionToAccountHandler(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IFileLoader fileLoader) :
    AbstractAccountSavingHandler<AddTransactionPriceHandler>(botClient, settingsPersistenceLayer, fileLoader),
    IConditionalStatusHandler
{
    private readonly ITelegramBotClientWrapper _botClient = botClient;
    public static string HandledStatus => "AddTransactionEnteringToAccount";
    protected override UserSettings TargetSetting => UserSettings.AccountTo;

    protected override async Task ContinueAfterSavingAccount(
        KMyMoneyFile file,
        Message message,
        CancellationToken cancellationToken)
    {
        await _botClient
            .Bot
            .SendMessage(
                message.Chat.Id,
                "Enter amount and, optionally, currency",
                cancellationToken: cancellationToken);
    }
}