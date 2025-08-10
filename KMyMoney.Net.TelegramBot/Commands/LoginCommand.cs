using System.Security.Cryptography;
using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands;

public class LoginCommand(
    ITelegramBotClientWrapper botWrapper,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IDropboxOAuth2HelperWrapper dropboxOAuth2HelperWrapper,
    IOptions<DropboxSettings> dropboxSettings) :
    ICommand
{
    public string Command => "login";
    public string Description => "Log in into Dropbox";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var state = RandomNumberGenerator.GetHexString(16);
        await settingsPersistenceLayer.SetSavedValueByKeyAsync(
            $"states/{state}",
            message.From!.Id.ToString(),
            TimeSpan.FromMinutes(10),
            cancellationToken: cancellationToken);

        var uri = dropboxOAuth2HelperWrapper.GetAuthorizeUri(
            OAuthResponseType.Code,
            clientId: dropboxSettings.Value.ApiKey,
            tokenAccessType: TokenAccessType.Online,
            state: state,
            redirectUri: dropboxSettings.Value.RedirectUri);

        await botWrapper.Bot.SendMessage(
            message.Chat.Id,
            $"Go here: {uri} to log in",
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }
}