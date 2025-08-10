using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.Telegram;
using Microsoft.Extensions.Options;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Commands;

public class LoginCommandTests
{
    [Fact]
    public async Task HandleAsync_ShouldSendLoginUrl()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var dropboxSettings = Options.Create(new DropboxSettings
        {
            ApiKey = "key",
            ApiSecret = "secret",
            RedirectUri = "https://redirect"
        });
        var command = new LoginCommand(botWrapper, settingsPersistenceLayer, dropboxOAuth2HelperWrapper, dropboxSettings);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        var authUri = new Uri("https://dropbox.com/oauth2/authorize");

        dropboxOAuth2HelperWrapper.GetAuthorizeUri(
            OAuthResponseType.Code,
            "key",
            "https://redirect",
            Arg.Any<string>(),
            tokenAccessType: TokenAccessType.Online)
            .Returns(authUri);

        // Act
        await command.HandleAsync(message, CancellationToken.None);

        // Assert
        await settingsPersistenceLayer.Received(1).SetSavedValueByKeyAsync(
            Arg.Is<string>(s => s.StartsWith("states/")),
            "123",
            TimeSpan.FromMinutes(10),
            CancellationToken.None);
        
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains(authUri.ToString())),
            CancellationToken.None);
    }
}
