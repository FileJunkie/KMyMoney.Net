using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.AddTransaction;

public class AddTransactionCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    ITelegramBotClientWrapper botClient,
    IFileLoader fileLoader) :
    AbstractMessageHandlerWithNextStep<AddTransactionFromAccountHandler>(settingsPersistenceLayer),
    ICommand
{
    public string Command => "add_transaction";
    public string Description => "Adds a new transaction";

    protected override async Task<bool> HandleInternalAsync(Message message, CancellationToken cancellationToken)
    {
        var file = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(
            message, cancellationToken);
        if (file == null)
        {
            return false;
        }

        var accounts = file
            .GetAccountsLatestTransactionDescending()
            .Select(acc => acc.Name);

        var keyboard = accounts.SplitBy(3);

        await botClient
            .Bot
            .SendMessage(
                message.Chat.Id,
                "Choose account to take money from",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        return true;
    }
}