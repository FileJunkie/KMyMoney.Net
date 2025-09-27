using System.Reflection;
using System.Text.Json;
using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Controllers;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Services;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KMyMoney.Net.TelegramBot.Tests.Controllers;

public class DropboxControllerTests
{
    [Fact]
    public async Task CallbackAsync_ShouldSaveToken_WhenStateAndCodeAreValid()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxSettings = Options.Create(new DropboxSettings
        {
            ApiKey = "key",
            ApiSecret = "secret",
            RedirectUri = "https://redirect"
        });
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var updateHandler = Substitute.For<IUpdateHandler>();
        var logger = Substitute.For<ILogger<DropboxController>>();
        var controller = new DropboxController(
            settingsPersistenceLayer,
            dropboxSettings,
            dropboxOAuth2HelperWrapper,
            updateHandler,
            logger);

        const string code = "valid_code";
        const string state = "valid_state";
        const long userId = 12345;

        var constructor = typeof(OAuth2Response).GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic)[0];
        var oauth2Response = (OAuth2Response)constructor.Invoke(
            [
                "access_token",
                "refresh_token",
                "uid",
                "state",
                "bearer",
                3600,
                new [] {"account_id"}
            ]);

        settingsPersistenceLayer.GetSavedValueByKeyAsync($"states/{state}").Returns(userId.ToString());
        dropboxOAuth2HelperWrapper.ProcessCodeFlowAsync(
            code,
            "key",
            "secret",
            "https://redirect")
            .Returns(oauth2Response);

        // Act
        var result = await controller.CallbackAsync(code, state, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(
            userId,
            UserSettings.Token,
            "access_token",
            Arg.Any<TimeSpan?>(),
            CancellationToken.None);
        await updateHandler.DidNotReceive().OnMessageAsync(
            Arg.Any<Message>(),
            Arg.Any<UpdateType>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallbackAsync_ShouldReprocessLastFailedMessage()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxSettings = Options.Create(new DropboxSettings
        {
            ApiKey = "key",
            ApiSecret = "secret",
            RedirectUri = "https://redirect"
        });
        var lastFailedMessage = new Message();
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            Arg.Any<long>(),
            UserSettings.LastFailedMessage,
            Arg.Any<CancellationToken>())
            .Returns(JsonSerializer.Serialize(lastFailedMessage));
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var updateHandler = Substitute.For<IUpdateHandler>();
        var logger = Substitute.For<ILogger<DropboxController>>();
        var controller = new DropboxController(
            settingsPersistenceLayer,
            dropboxSettings,
            dropboxOAuth2HelperWrapper,
            updateHandler,
            logger);

        var updateHandlerCalled = false;
        updateHandler
            .WhenForAnyArgs(h => h
                .OnMessageAsync(
                    Arg.Any<Message>(),
                    Arg.Any<UpdateType>(),
                    Arg.Any<CancellationToken>()))
            .Do(_ => updateHandlerCalled = true);

        const string code = "valid_code";
        const string state = "valid_state";
        const long userId = 12345;

        var constructor = typeof(OAuth2Response).GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic)[0];
        var oauth2Response = (OAuth2Response)constructor.Invoke(
            [
                "access_token",
                "refresh_token",
                "uid",
                "state",
                "bearer",
                3600,
                new [] {"account_id"}
            ]);

        settingsPersistenceLayer.GetSavedValueByKeyAsync($"states/{state}").Returns(userId.ToString());
        dropboxOAuth2HelperWrapper.ProcessCodeFlowAsync(
            code,
            "key",
            "secret",
            "https://redirect")
            .Returns(oauth2Response);

        // Act
        var result = await controller.CallbackAsync(code, state, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(
            userId,
            UserSettings.Token,
            "access_token",
            Arg.Any<TimeSpan?>(),
            CancellationToken.None);
        for (var i = 0; i < 10; i++)
        {
            if (!updateHandlerCalled)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            else
            {
                break;
            }
        }
        updateHandlerCalled.ShouldBeTrue();
        await updateHandler.Received(1).OnMessageAsync(
            Arg.Any<Message>(),
            Arg.Any<UpdateType>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallbackAsync_ShouldReturnBadRequest_WhenStateIsInvalid()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxSettings = Options.Create(new DropboxSettings
        {
            ApiKey = "key",
            ApiSecret = "secret"
        });
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var updateHandler = Substitute.For<IUpdateHandler>();
        var logger = Substitute.For<ILogger<DropboxController>>();
        var controller = new DropboxController(
            settingsPersistenceLayer,
            dropboxSettings,
            dropboxOAuth2HelperWrapper,
            updateHandler,
            logger);

        settingsPersistenceLayer.GetSavedValueByKeyAsync(Arg.Any<string>()).Returns((string?)null);

        // Act
        var result = await controller.CallbackAsync("any_code", "invalid_state", CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BadRequestResult>();
        await updateHandler.DidNotReceive().OnMessageAsync(
            Arg.Any<Message>(),
            Arg.Any<UpdateType>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CallbackAsync_ShouldReturnForbid_WhenTokenIsNull()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var dropboxSettings = Options.Create(new DropboxSettings
        {
            ApiKey = "key",
            ApiSecret = "secret",
            RedirectUri = "https://redirect"
        });
        var dropboxOAuth2HelperWrapper = Substitute.For<IDropboxOAuth2HelperWrapper>();
        var updateHandler = Substitute.For<IUpdateHandler>();
        var logger = Substitute.For<ILogger<DropboxController>>();
        var controller = new DropboxController(
            settingsPersistenceLayer,
            dropboxSettings,
            dropboxOAuth2HelperWrapper,
            updateHandler,
            logger);

        const string code = "valid_code";
        const string state = "valid_state";
        const long userId = 12345;

        settingsPersistenceLayer.GetSavedValueByKeyAsync($"states/{state}").Returns(userId.ToString());
        dropboxOAuth2HelperWrapper.ProcessCodeFlowAsync(
            code,
            "key",
            "secret",
            "https://redirect")
            .Returns((OAuth2Response?)null);

        // Act
        var result = await controller.CallbackAsync(code, state, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ForbidResult>();
        await updateHandler.DidNotReceive().OnMessageAsync(
            Arg.Any<Message>(),
            Arg.Any<UpdateType>(),
            Arg.Any<CancellationToken>());
    }
}
