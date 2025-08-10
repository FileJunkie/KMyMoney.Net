using KMyMoney.Net.TelegramBot.Controllers;
using KMyMoney.Net.TelegramBot.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UpdateHandler = KMyMoney.Net.TelegramBot.Services.IUpdateHandler;

namespace KMyMoney.Net.TelegramBot.Tests.Controllers;

public class WebhookControllerTests
{
    [Fact]
    public async Task Post_ShouldCallUpdateHandler_WhenTokenIsValid()
    {
        // Arrange
        var updateHandler = Substitute.For<UpdateHandler>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var logger = Substitute.For<ILogger<WebhookController>>();
        var controller = new WebhookController(updateHandler, settingsPersistenceLayer, logger);

        const string secretToken = "valid_token";
        var update = new Update { Message = new () };
        settingsPersistenceLayer.GetSavedValueByKeyAsync("telegramSecret").Returns(secretToken);

        // Act
        var result = await controller.Post(update, secretToken, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();
        await updateHandler.Received(1).OnMessageAsync(update.Message, UpdateType.Message, CancellationToken.None);
    }

    [Fact]
    public async Task Post_ShouldReturnForbid_WhenTokenIsInvalid()
    {
        // Arrange
        var updateHandler = Substitute.For<UpdateHandler>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var logger = Substitute.For<ILogger<WebhookController>>();
        var controller = new WebhookController(updateHandler, settingsPersistenceLayer, logger);

        settingsPersistenceLayer.GetSavedValueByKeyAsync("telegramSecret").Returns("expected_token");

        // Act
        var result = await controller.Post(new Update(), "invalid_token", CancellationToken.None);

        // Assert
        result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Post_ShouldCallErrorHandler_WhenUpdateHandlerThrows()
    {
        // Arrange
        var updateHandler = Substitute.For<UpdateHandler>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var logger = Substitute.For<ILogger<WebhookController>>();
        var controller = new WebhookController(updateHandler, settingsPersistenceLayer, logger);

        const string secretToken = "valid_token";
        var update = new Update { Message = new Message() };
        var exception = new InvalidOperationException("test");
        settingsPersistenceLayer.GetSavedValueByKeyAsync("telegramSecret").Returns(secretToken);
        updateHandler.OnMessageAsync(Arg.Any<Message>(), Arg.Any<UpdateType>(), Arg.Any<CancellationToken>())
            .Throws(exception);

        // Act
        var result = await controller.Post(update, secretToken, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();
        await updateHandler.Received(1).OnErrorAsync(exception, HandleErrorSource.HandleUpdateError);
    }

    [Fact]
    public async Task Post_ShouldDoNothing_WhenUpdateHasNoMessage()
    {
        // Arrange
        var updateHandler = Substitute.For<UpdateHandler>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var logger = Substitute.For<ILogger<WebhookController>>();
        var controller = new WebhookController(updateHandler, settingsPersistenceLayer, logger);

        const string secretToken = "valid_token";
        var update = new Update { Message = null };
        settingsPersistenceLayer.GetSavedValueByKeyAsync("telegramSecret").Returns(secretToken);

        // Act
        var result = await controller.Post(update, secretToken, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();
        await updateHandler.DidNotReceiveWithAnyArgs().OnMessageAsync(null!, default, CancellationToken.None);
    }
}