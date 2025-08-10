using KMyMoney.Net.TelegramBot.Commands.File;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Commands.File;

public class FileEntryStatusHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldSavePath_WhenPathIsProvided()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var handler = new FileEntryStatusHandler(botWrapper, settingsPersistenceLayer);

        var message = new Message 
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "test.kmy" };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(123, 
            UserSettings.FilePath, "/test.kmy", 
            cancellationToken: CancellationToken.None);
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(
                r => r.Text.Contains("Got your file path, saving")), 
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldSendError_WhenPathIsEmpty()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var handler = new FileEntryStatusHandler(botWrapper, settingsPersistenceLayer);

        var message = new Message 
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "" };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Empty path, really?")), 
            CancellationToken.None);
    }
}