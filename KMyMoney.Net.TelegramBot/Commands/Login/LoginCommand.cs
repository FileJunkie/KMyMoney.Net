using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Commands.Login;

public class LoginCommand(
    TelegramBotClientWrapper botWrapper,
    ISettingsPersistenceLayer settingsLayer,
    LoginCodeEntryStatusHandler loginCodeEntryStatusHandler,
    IOptions<DropboxSettings> dropboxSettings) :
    ICommand
{
    public string Command => "login";
    public string Description => "Log in into Dropbox";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var uri = DropboxOAuth2Helper.GetAuthorizeUri(
            OAuthResponseType.Code,
            clientId: dropboxSettings.Value.ApiKey,
            tokenAccessType: TokenAccessType.Legacy,
            redirectUri: (string?)null);

        await settingsLayer.SetStatusByUserIdAsync(
            message.From!.Id,
            loginCodeEntryStatusHandler.HandledStatus,
            cancellationToken);

        await botWrapper.Bot.SendMessage(
            message.Chat.Id,
            $"Go here: {uri} and tell me the code",
            cancellationToken: cancellationToken);
    }
}