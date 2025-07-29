using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TgBotFramework;

namespace KMyMoney.Net.TelegramBot.Commands;

public class LoginCommand(IOptions<DropboxSettings> dropboxSettings) :
    CommandBase<UpdateContext>
{
    public override async Task HandleAsync(
        UpdateContext context,
        UpdateDelegate<UpdateContext> next,
        string[] args,
        CancellationToken cancellationToken)
    {
        var uri = DropboxOAuth2Helper.GetAuthorizeUri(
            OAuthResponseType.Code,
            clientId: dropboxSettings.Value.ApiKey,
            tokenAccessType: TokenAccessType.Legacy,
            redirectUri: (string?)null);

        await context.Client.SendTextMessageAsync(
            context.Chat.Id,
            $"Go here: {uri} and call /logincode to enter the code",
            cancellationToken: cancellationToken);
    }
}