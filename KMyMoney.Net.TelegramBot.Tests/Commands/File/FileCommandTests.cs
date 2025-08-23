using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.TelegramBot.Commands.File;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Commands.File;

public class FileCommandTests
{
    [Fact]
    public async Task HandleAsync_ShouldPromptForFile_WhenTokenExists()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileEntryStatusHandler = new FileEntryStatusHandler(null!, null!);
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        var fileAccessorFactory = Substitute.For<IFileAccessService>();
        botWrapper.Bot.Returns(botClient);
        var command = new FileCommand(
            settingsPersistenceLayer,
            fileEntryStatusHandler,
            fileAccessorFactory,
            botWrapper);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string token = "valid_token";

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Token)
            .Returns(token);

        var fileAccessor = Substitute.For<IFileAccessor>();
        fileAccessorFactory.CreateFileAccessor(Arg.Any<string>()).Returns(fileAccessor);
        fileAccessor.ListFilesAsync().Returns(["file1.kmy", "file2.kmy"]);

        // Act
        await command.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Choose .kmy file")),
            CancellationToken.None);
        fileAccessorFactory.Received(1).CreateFileAccessor(Arg.Any<string>());
        await fileAccessor.Received(1).ListFilesAsync();
    }

    [Fact]
    public async Task HandleAsync_ShouldSendError_WhenTokenIsMissing()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileEntryStatusHandler = new FileEntryStatusHandler(null!, null!);
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        var fileAccessorFactory = Substitute.For<IFileAccessService>();
        botWrapper.Bot.Returns(botClient);
        var command = new FileCommand(settingsPersistenceLayer,
            fileEntryStatusHandler, fileAccessorFactory, botWrapper);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Token)
            .Returns((string?)null);

        // Act
        await command.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(
                r => r.Text.Contains("Log in with /login command first")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldSendError_WhenNoFilesExist()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileEntryStatusHandler = new FileEntryStatusHandler(null!, null!);
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        var fileAccessorFactory = Substitute.For<IFileAccessService>();
        botWrapper.Bot.Returns(botClient);
        var command = new FileCommand(settingsPersistenceLayer,
            fileEntryStatusHandler, fileAccessorFactory, botWrapper);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string token = "valid_token";

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Token)
            .Returns(token);

        var fileAccessor = Substitute.For<IFileAccessor>();
        fileAccessorFactory.CreateFileAccessor(Arg.Any<string>()).Returns(fileAccessor);
        fileAccessor.ListFilesAsync().Returns([]);

        // Act
        await command.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(
                r => r.Text.Contains("You have no .kmy files, mate")),
            CancellationToken.None);
    }
}