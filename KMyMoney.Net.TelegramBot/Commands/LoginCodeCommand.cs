using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TgBotFramework;

namespace KMyMoney.Net.TelegramBot.Commands;

public class LoginCodeCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IOptions<DropboxSettings> dropboxSettings) :
    CommandBase<UpdateContext>
{
    public override async Task HandleAsync(
        UpdateContext context,
        UpdateDelegate<UpdateContext> next,
        string[] args,
        CancellationToken cancellationToken)
    {
        await context.Client.DeleteMessageAsync(context.Chat.Id, context.Update.Message!.MessageId, cancellationToken);
        await context.Client.SendTextMessageAsync(
            context.Chat.Id,
            "Got your code, getting and saving token",
            cancellationToken: cancellationToken);
        var token = await DropboxOAuth2Helper.ProcessCodeFlowAsync(
            args.Single(),
            dropboxSettings.Value.ApiKey,
            dropboxSettings.Value.ApiSecret);

        if (token == null)
        {
            await context.Client.SendTextMessageAsync(
                context.Chat.Id,
                "Something went wrong with getting token",
                cancellationToken: cancellationToken);
            return;
        }

        await settingsPersistenceLayer.SetTokenByUserIdAsync(context.Sender.Id, token.AccessToken);
    }
}