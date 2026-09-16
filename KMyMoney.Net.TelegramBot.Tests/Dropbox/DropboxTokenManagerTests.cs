using System.Reflection;
using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;

namespace KMyMoney.Net.TelegramBot.Tests.Dropbox;

public class DropboxTokenManagerTests
{
    private const long UserId = 12345;

    private static OAuth2Response CreateOAuth2Response(
        string accessToken = "new_access_token",
        string refreshToken = "refresh_token")
    {
        var constructor = typeof(OAuth2Response).GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic)[0];
        return (OAuth2Response)constructor.Invoke(
            [
                accessToken,
                refreshToken,
                "uid",
                "state",
                "bearer",
                3600,
                new[] { "account_id" }
            ]);
    }

    private static IOptions<DropboxSettings> CreateDropboxSettings() =>
        Options.Create(new DropboxSettings
        {
            ApiKey = "key",
            ApiSecret = "secret",
            RedirectUri = "https://redirect"
        });

    private class TestableTokenManager(
        ISettingsPersistenceLayer settingsPersistenceLayer,
        IDropboxOAuth2HelperWrapper dropboxOAuth2HelperWrapper,
        IOptions<DropboxSettings> dropboxSettings,
        ILogger<DropboxTokenManager>? logger = null,
        bool tokenValid = true) : DropboxTokenManager(
            settingsPersistenceLayer,
            dropboxOAuth2HelperWrapper,
            dropboxSettings,
            logger)
    {
        private readonly bool _tokenValid = tokenValid;

        protected override Task<bool> IsTokenValidAsync(
            string accessToken,
            CancellationToken cancellationToken) =>
            Task.FromResult(_tokenValid);
    }

    [Fact]
    public async Task GetValidAccessTokenAsync_WhenAccessTokenMissingButRefreshTokenExists_ShouldRefreshAndReturnNewToken()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var logger = Substitute.For<ILogger<DropboxTokenManager>>();

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.Token, Arg.Any<CancellationToken>())
            .Returns((string?)null);
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.RefreshToken, Arg.Any<CancellationToken>())
            .Returns("stored_refresh_token");

        var oauth2Response = CreateOAuth2Response();
        dropboxOAuth2HelperWrapper.ProcessRefreshFlowAsync(
            "stored_refresh_token", "key", "secret", "https://redirect")
            .Returns(oauth2Response);

        var manager = new TestableTokenManager(
            settingsPersistenceLayer,
            dropboxOAuth2HelperWrapper,
            CreateDropboxSettings(),
            logger);

        // Act
        var result = await manager.GetValidAccessTokenAsync(UserId, CancellationToken.None);

        // Assert
        result.ShouldBe("new_access_token");
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(
            UserId,
            UserSettings.Token,
            "new_access_token",
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetValidAccessTokenAsync_WhenBothTokensMissing_ShouldThrowTokenRefreshFailedException()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var logger = Substitute.For<ILogger<DropboxTokenManager>>();

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.Token, Arg.Any<CancellationToken>())
            .Returns((string?)null);
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.RefreshToken, Arg.Any<CancellationToken>())
            .Returns((string?)null);

        var manager = new TestableTokenManager(
            settingsPersistenceLayer,
            dropboxOAuth2HelperWrapper,
            CreateDropboxSettings(),
            logger);

        // Act
        var action = () => manager.GetValidAccessTokenAsync(UserId, CancellationToken.None);

        // Assert
        var ex = await action.ShouldThrowAsync<TokenRefreshFailedException>();
        ex.Message.ShouldContain("Please use /login");
        await dropboxOAuth2HelperWrapper.DidNotReceive().ProcessRefreshFlowAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>());
    }

    [Fact]
    public async Task GetValidAccessTokenAsync_WhenAccessTokenValid_ShouldReturnWithoutRefresh()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var logger = Substitute.For<ILogger<DropboxTokenManager>>();

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.Token, Arg.Any<CancellationToken>())
            .Returns("valid_access_token");

        var manager = new TestableTokenManager(
            settingsPersistenceLayer,
            dropboxOAuth2HelperWrapper,
            CreateDropboxSettings(),
            logger,
            tokenValid: true);

        // Act
        var result = await manager.GetValidAccessTokenAsync(UserId, CancellationToken.None);

        // Assert
        result.ShouldBe("valid_access_token");
        await dropboxOAuth2HelperWrapper.DidNotReceive().ProcessRefreshFlowAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string?>());
    }

    [Fact]
    public async Task GetValidAccessTokenAsync_WhenAccessTokenInvalidButRefreshTokenExists_ShouldRefreshAndReturnNewToken()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var logger = Substitute.For<ILogger<DropboxTokenManager>>();

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.Token, Arg.Any<CancellationToken>())
            .Returns("expired_access_token");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.RefreshToken, Arg.Any<CancellationToken>())
            .Returns("stored_refresh_token");

        var oauth2Response = CreateOAuth2Response();
        dropboxOAuth2HelperWrapper.ProcessRefreshFlowAsync(
            "stored_refresh_token", "key", "secret", "https://redirect")
            .Returns(oauth2Response);

        var manager = new TestableTokenManager(
            settingsPersistenceLayer,
            dropboxOAuth2HelperWrapper,
            CreateDropboxSettings(),
            logger,
            tokenValid: false);

        // Act
        var result = await manager.GetValidAccessTokenAsync(UserId, CancellationToken.None);

        // Assert
        result.ShouldBe("new_access_token");
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(
            UserId,
            UserSettings.Token,
            "new_access_token",
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetValidAccessTokenAsync_WhenRefreshFails_ShouldThrowTokenRefreshFailedException()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var logger = Substitute.For<ILogger<DropboxTokenManager>>();

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.Token, Arg.Any<CancellationToken>())
            .Returns("expired_access_token");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            UserId, UserSettings.RefreshToken, Arg.Any<CancellationToken>())
            .Returns("stored_refresh_token");

        dropboxOAuth2HelperWrapper.ProcessRefreshFlowAsync(
            "stored_refresh_token", "key", "secret", "https://redirect")
            .Returns(Task.FromException<OAuth2Response?>(new HttpRequestException("400")));

        var manager = new TestableTokenManager(
            settingsPersistenceLayer,
            dropboxOAuth2HelperWrapper,
            CreateDropboxSettings(),
            logger,
            tokenValid: false);

        // Act
        var action = () => manager.GetValidAccessTokenAsync(UserId, CancellationToken.None);

        // Assert
        await action.ShouldThrowAsync<TokenRefreshFailedException>();
    }
}
