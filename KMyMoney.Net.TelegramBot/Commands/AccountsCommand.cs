using System.Text;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands;

public class AccountsCommand(
    ITelegramBotClientWrapper botClient,
    IFileLoader fileLoader) :
    ICommand
{
    public string Command => "accounts";
    public string Description => "Get accounts from .kmy file";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var file = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(
            message, cancellationToken);
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
                await botClient.Bot.SendMessageAsync(message.Chat.Id, answer,
                    cancellationToken: cancellationToken);
                sb.Clear();
            }
        }

        if (sb.Length > 0)
        {
            await botClient.Bot.SendMessageAsync(message.Chat.Id, sb.ToString(),
                cancellationToken: cancellationToken);
        }
    }
}