using System.Text;
using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.StatusHandlers;

public class DefaultStatusHandler(
    ITelegramBotClientWrapper botClientWrapper,
    IEnumerable<ICommand> commands) : IDefaultStatusHandler
{
    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var command = ExtractCommand(message.Text);

        if (!string.IsNullOrWhiteSpace(command))
        {
            foreach (var cmd in commands)
            {
                if (cmd.Command == command)
                {
                    await cmd.HandleAsync(message, cancellationToken);
                    return;
                }
            }
        }

        await PrintHelpAsync(message.Chat.Id);
    }

    private async Task PrintHelpAsync(ChatId chatId)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("I don't exactly understand you, but I support the following commands:");
        foreach (var cmd in commands)
        {
            stringBuilder.AppendLine($"/{cmd.Command}: {cmd.Description}");
        }

        await botClientWrapper.Bot.SendMessageAsync(chatId, stringBuilder.ToString());
    }

    private static string? ExtractCommand(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        return !message.StartsWith('/') ?
            null :
            message[1..].Split(' ')[0];
    }
}