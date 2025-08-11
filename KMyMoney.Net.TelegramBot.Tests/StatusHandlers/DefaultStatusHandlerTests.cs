using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.StatusHandlers;

public class DefaultStatusHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldExecuteCommand_WhenCommandExists()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);

        var command = Substitute.For<ICommand>();
        command.Command.Returns("test");
        var commands = new[] { command };

        var handler = new DefaultStatusHandler(botWrapper, commands);
        var message = new Message { Text = "/test with args", Chat = new Chat { Id = 123 } };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await command.Received(1).HandleAsync(message, CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldPrintHelp_WhenCommandDoesNotExist()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);

        var handler = new DefaultStatusHandler(botWrapper, []);
        var message = new Message { Text = "/unknown", Chat = new Chat { Id = 123 } };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(Arg.Any<SendMessageRequest>(), CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldPrintHelp_WhenMessageIsNotACommand()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);

        var handler = new DefaultStatusHandler(botWrapper, []);
        var message = new Message { Text = "just some text", Chat = new Chat { Id = 123 } };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(Arg.Any<SendMessageRequest>(), CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldPrintHelp_WhenMessageTextIsEmpty()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);

        var handler = new DefaultStatusHandler(botWrapper, []);
        var message = new Message { Text = "", Chat = new Chat { Id = 123 } };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(Arg.Any<SendMessageRequest>(), CancellationToken.None);
    }
}
