using System.Text;
using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot;
using TgBotFramework;

namespace KMyMoney.Net.TelegramBot.Commands;

public class AccountsCommand(ISettingsPersistenceLayer settingsPersistenceLayer) :
    CommandBase<UpdateContext>
{
    public override async Task HandleAsync(
        UpdateContext context,
        UpdateDelegate<UpdateContext> next,
        string[] args,
        CancellationToken cancellationToken)
    {
        var token = await settingsPersistenceLayer.GetTokenByUserIdAsync(context.Sender.Id);
        if (string.IsNullOrWhiteSpace(token))
        {
            await context.Client.SendTextMessageAsync(
                context.Chat.Id,
                "Use /login + /logincode to set access token",
                cancellationToken: cancellationToken);
            return;
        }

        var filePath = await settingsPersistenceLayer.GetFilePathByUserIdAsync(context.Sender.Id);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            await context.Client.SendTextMessageAsync(context.Chat.Id, "Use /file to set file path",
                cancellationToken: cancellationToken);
            return;
        }

        await context.Client.SendTextMessageAsync(context.Chat.Id, $"Wait, let me load the file",
            cancellationToken: cancellationToken);
        var fileAccessor = new DropboxFileAccessor(token);
        var fileUri = new Uri($"dropbox://{filePath}");
        var fileLoader = new KMyMoneyLoaderBuilder()
            .WithFileAccessor(fileAccessor)
            .Build();
        var file = await fileLoader.LoadFileAsync(fileUri);
        await context.Client.SendTextMessageAsync(context.Chat.Id, $"Loaded", cancellationToken: cancellationToken);

        var sb = new StringBuilder();
        foreach (var (account, i) in file.Root.Accounts.Values.Select((x, y) => (x, y)))
        {
            sb.AppendLine($"Id: {account.Id} name: {account.Name}");
            if (i % 10 == 0)
            {
                var answer = sb.ToString();
                await context.Client.SendTextMessageAsync(context.Chat.Id, answer,
                    cancellationToken: cancellationToken);
                sb.Clear();
            }
        }

        if (sb.Length > 0)
        {
            await context.Client.SendTextMessageAsync(context.Chat.Id, sb.ToString(),
                cancellationToken: cancellationToken);
        }
    }
}