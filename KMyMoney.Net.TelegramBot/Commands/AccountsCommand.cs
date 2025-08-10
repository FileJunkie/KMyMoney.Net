using System.Text;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.TelegramBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands;

public class AccountsCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    ITelegramBotClientWrapper botClient) :
    ICommand
{
    public string Command => "accounts";
    public string Description => "Get accounts from .kmy file";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var file = await FileLoaderHelpers.LoadKMyMoneyFileOrSendErrorAsync(
            settingsPersistenceLayer, botClient.Bot, message, cancellationToken);
        if (file == null)
        {
            return;
        }

        var sb = new StringBuilder();
        foreach (var (account, i) in file
                     .Root
                     .Accounts
                     .Values
                     .Where(acc => !acc.IsClosed)
                     .Select((x, y) => (x, y)))
        {
            sb.AppendLine($"Id: {account.Id} name: {account.Name}");
            if (i % 10 == 0)
            {
                var answer = sb.ToString();
                await botClient.Bot.SendMessage(message.Chat.Id, answer,
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
                sb.Clear();
            }
        }

        if (sb.Length > 0)
        {
            await botClient.Bot.SendMessage(message.Chat.Id, sb.ToString(),
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }
    }
}