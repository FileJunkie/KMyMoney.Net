using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Services;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KMyMoney.Net.TelegramBot.Tests.Services;

public class UpdateHandlerTests
{
    [Fact]
    public async Task OnMessageAsync_ShouldRouteToConditionalHandler_WhenStatusIsSet()
    {
        // Arrange
        const string userStatus = "TestStatus";
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var statusHandler = Substitute.For<IStatusHandler>();
        var conditionalStatusHandlerDescriptor = new ConditionalStatusHandlerDescriptor(
            statusHandler,
            userStatus);
        var defaultStatusHandler = Substitute.For<ICommandDispatcher>();
        var logger = Substitute.For<ILogger<UpdateHandler>>();
        var handler = new UpdateHandler(botWrapper, settingsPersistenceLayer,
            [conditionalStatusHandlerDescriptor], defaultStatusHandler, logger);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456, Type = ChatType.Private } };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Status)
            .Returns(userStatus);

        // Act
        await handler.OnMessageAsync(message, UpdateType.Message, CancellationToken.None);

        // Assert
        await statusHandler.Received(1)
            .HandleAsync(message, CancellationToken.None);
        await defaultStatusHandler.DidNotReceive()
            .HandleAsync(message, CancellationToken.None);
    }

    [Fact]
    public async Task OnMessageAsync_ShouldRouteToDefaultHandler_WhenStatusIsNotSet()
    {
        // Arrange
        const string userStatus = "TestStatus";
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var statusHandler = Substitute.For<IStatusHandler>();
        var conditionalStatusHandlerDescriptor = new ConditionalStatusHandlerDescriptor(
            statusHandler,
            userStatus);
        var defaultStatusHandler = Substitute.For<ICommandDispatcher>();
        var logger = Substitute.For<ILogger<UpdateHandler>>();
        var handler = new UpdateHandler(botWrapper, settingsPersistenceLayer,
            [conditionalStatusHandlerDescriptor], defaultStatusHandler, logger);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456, Type = ChatType.Private } };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Status)
            .Returns((string?)null);

        // Act
        await handler.OnMessageAsync(message, UpdateType.Message, CancellationToken.None);

        // Assert
        await defaultStatusHandler.Received(1)
            .HandleAsync(message, CancellationToken.None);
    }

    [Fact]
    public async Task OnMessageAsync_ShouldSendError_WhenHandlerThrows()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var defaultStatusHandler = Substitute.For<ICommandDispatcher>();
        var logger = Substitute.For<ILogger<UpdateHandler>>();
        var handler = new UpdateHandler(botWrapper, settingsPersistenceLayer,
            [], defaultStatusHandler, logger);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456, Type = ChatType.Private } };

        defaultStatusHandler.HandleAsync(message, CancellationToken.None)
            .Throws(new Exception("Test exception"));

        // Act
        await handler.OnMessageAsync(message, UpdateType.Message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Sorry, mate")),
            CancellationToken.None);
    }

    [Theory]
    [InlineData(UpdateType.CallbackQuery)]
    [InlineData(UpdateType.ChannelPost)]
    public async Task OnMessageAsync_ShouldIgnore_NonMessageUpdates(
        UpdateType updateType)
    {
        // Arrange
        var defaultStatusHandler = Substitute.For<ICommandDispatcher>();
        var logger = Substitute.For<ILogger<UpdateHandler>>();
        var handler = new UpdateHandler(null!, null!, null!, defaultStatusHandler,
            logger);
        var message = new Message();

        // Act
        await handler.OnMessageAsync(message, updateType, CancellationToken.None);

        // Assert
        await defaultStatusHandler.DidNotReceiveWithAnyArgs()
            .HandleAsync(null!, CancellationToken.None);
    }

    [Fact]
    public async Task OnMessageAsync_ShouldIgnore_GroupMessages()
    {
        // Arrange
        var defaultStatusHandler = Substitute.For<ICommandDispatcher>();
        var logger = Substitute.For<ILogger<UpdateHandler>>();
        var handler = new UpdateHandler(null!, null!, null!, defaultStatusHandler,
            logger);
        var message = new Message { Chat = new Chat { Type = ChatType.Group } };

        // Act
        await handler.OnMessageAsync(message, UpdateType.Message,
            CancellationToken.None);

        // Assert
        await defaultStatusHandler.DidNotReceiveWithAnyArgs()
            .HandleAsync(null!, CancellationToken.None);
    }

    [Fact]
    public async Task OnMessageAsync_ShouldIgnore_MessagesWithNoFrom()
    {
        // Arrange
        var defaultStatusHandler = Substitute.For<ICommandDispatcher>();
        var logger = Substitute.For<ILogger<UpdateHandler>>();
        var handler = new UpdateHandler(null!, null!, null!, defaultStatusHandler,
            logger);
        var message = new Message
            { From = null, Chat = new Chat { Type = ChatType.Private } };

        // Act
        await handler.OnMessageAsync(message, UpdateType.Message,
            CancellationToken.None);

        // Assert
        await defaultStatusHandler.DidNotReceiveWithAnyArgs()
            .HandleAsync(null!, CancellationToken.None);
    }
}
