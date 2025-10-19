using System.Text;
using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands;

public class AccountsCommand(
    ITelegramBotClientWrapper botClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IFileLoader fileLoader) :
    AbstractMessageHandler(botClient, settingsPersistenceLayer), ICommand
{
    private readonly ITelegramBotClientWrapper _botClient = botClient;
    public string Command => "accounts";
    public string Description => "Get accounts from .kmy file";

    protected override async Task HandleAfterResettingStatusAsync(Message message, CancellationToken cancellationToken)
    {
        var file = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(
            message, cancellationToken);

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
                await _botClient.Bot.SendMessageAsync(message.Chat.Id, answer,
                    cancellationToken: cancellationToken);
                sb.Clear();
            }
        }

        if (sb.Length > 0)
        {
            await _botClient.Bot.SendMessageAsync(message.Chat.Id, sb.ToString(),
                cancellationToken: cancellationToken);
        }
    }
}