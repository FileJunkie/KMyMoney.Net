using System.Text;
using Dropbox.Api;
using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KMyMoney.Net.TelegramBot;

public class TelegramService(
    TelegramBotClient telegramBotClient,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    ILogger<TelegramService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        telegramBotClient.OnMessage += OnMessageAsync;
        telegramBotClient.OnError += OnErrorAsync;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    private async Task OnMessageAsync(Message message, UpdateType type)
    {
        if (type != UpdateType.Message)
        {
            logger.LogInformation("What is {Type}?", type);
            return;
        }

        if (message.Text == null)
        {
            logger.LogInformation("No text received");
            return;
        }

        if (message.Chat.Type != ChatType.Private)
        {
            logger.LogWarning("Are they hacking me with {Type}?", message.Chat.Type);
            return;
        }

        if (message.From == null)
        {
            logger.LogWarning("No user?");
            return;
        }

        if (message.Text.StartsWith("/login"))
        {
            await StartLoginAsync(message);
            return;
        }

        if (message.Text.StartsWith("/code"))
        {
            await FinishLoginAsync(message);
            return;
        }

        if (message.Text.StartsWith("/accounts"))
        {
            await ListAccountAsync(message);
            return;
        }

        await telegramBotClient.SendMessage(message.Chat.Id, "You okay bro?");
    }

    private async Task StartLoginAsync(Message message)
    {
        var uri = DropboxOAuth2Helper.GetAuthorizeUri(
            OAuthResponseType.Code,
            clientId: Environment.GetEnvironmentVariable("DROPBOX_API_KEY"), // TODO
            tokenAccessType: TokenAccessType.Legacy,
            redirectUri: (string?)null);
        await telegramBotClient.SendMessage(message.Chat.Id, $"Go here: {uri}");
    }

    private async Task FinishLoginAsync(Message message)
    {
        await telegramBotClient.DeleteMessage(message.Chat.Id, message.MessageId);
        var code = message.Text!.Replace("/code ", string.Empty);
        var token = await DropboxOAuth2Helper.ProcessCodeFlowAsync(
            code,
            Environment.GetEnvironmentVariable("DROPBOX_API_KEY"), // TODO,
            Environment.GetEnvironmentVariable("DROPBOX_API_SECRET")); // TODO
        if (token == null)
        {
            await telegramBotClient.SendMessage(message.Chat.Id, "Naaaaah");
            return;
        }

        await telegramBotClient.SendMessage(message.Chat.Id, $"Okay whatever");
        settingsPersistenceLayer.SetTokenByUserId(message.From!.Id, token.AccessToken);
    }

    private async Task ListAccountAsync(Message message)
    {
        if (!settingsPersistenceLayer.TryGetTokenByUserId(message.From!.Id, out var token))
        {
            await telegramBotClient.SendMessage(message.Chat.Id, "You fine bro?");
            return;
        }

        await telegramBotClient.SendMessage(message.Chat.Id, $"Wait, let me load the file");
        var fileAccessor = new DropboxFileAccessor(token);
        var fileUri = new Uri($"dropbox://{message.Text!.Replace("/accounts ", string.Empty)}");
        var fileLoader = new KMyMoneyLoaderBuilder()
            .WithFileAccessor(fileAccessor)
            .Build();
        var file = await fileLoader.LoadFileAsync(fileUri);
        await telegramBotClient.SendMessage(message.Chat.Id, $"Loaded");

        var sb = new StringBuilder();
        foreach (var (account, i) in file.Root.Accounts.Values.Select((x, y) => (x, y)))
        {
            sb.AppendLine($"Id: {account.Id} name: {account.Name}");
            if (i % 10 == 0)
            {
                var answer = sb.ToString();
                await telegramBotClient.SendMessage(message.Chat.Id, answer);
                sb.Clear();
            }
        }

        if (sb.Length > 0)
        {
            await telegramBotClient.SendMessage(message.Chat.Id, sb.ToString());
        }
    }

    private Task OnErrorAsync(Exception exception, HandleErrorSource source)
    {
        logger.LogError(exception, exception.Message);
        return Task.CompletedTask;
    }
}