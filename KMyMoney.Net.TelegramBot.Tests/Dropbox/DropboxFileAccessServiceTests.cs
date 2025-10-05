using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using NSubstitute;
using Shouldly;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KMyMoney.Net.TelegramBot.Tests.Dropbox;

public class DropboxFileAccessServiceTests
{
    [Fact]
    public async Task CreateFileAccessorAsync_WhenTokenExists_ShouldReturnDropboxFileAccessor()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var botClientWrapper = Substitute.For<ITelegramBotClientWrapper>();
        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string token = "test_token";
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(message.From.Id, UserSettings.Token, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>(token));
        var service = new DropboxFileAccessService(settingsPersistenceLayer, botClientWrapper);

        // Act
        var result = await service.CreateFileAccessorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<DropboxFileAccessor>();
    }

    [Fact]
    public async Task CreateFileAccessorAsync_WhenTokenIsMissing_ShouldReturnNullAndSendMessage()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var message = new Message
        {
            From = new User { Id = 123 },
            Chat = new Chat
            {
                Id = 456,
                Type = ChatType.Private,
            }
        };
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(message.From.Id, UserSettings.Token, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>(null));
        var service = new DropboxFileAccessService(settingsPersistenceLayer, Substitute.For<ITelegramBotClientWrapper>());

        // Act
        var result = await service.CreateFileAccessorAsync(
            message,
            CancellationToken.None);

        // Assert
        result.ShouldBeNull();
        await settingsPersistenceLayer
            .Received(1)
            .SetUserSettingByUserIdAsync(
                message.From.Id,
                UserSettings.LastFailedMessage,
                Arg.Any<string?>(),
                TimeSpan.FromMinutes(15),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetFilePathAsync_WhenFilePathExists_ShouldReturnFilePath()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var botClientWrapper = Substitute.For<ITelegramBotClientWrapper>();
        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string filePath = "/test/file.kmy";
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(message.From.Id, UserSettings.FilePath, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>(filePath));
        var service = new DropboxFileAccessService(settingsPersistenceLayer, botClientWrapper);

        // Act
        var result = await service.GetFilePathAsync(message, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(filePath);
    }

    [Fact]
    public async Task GetFilePathAsync_WhenFilePathIsMissing_ShouldReturnNullAndSendMessage()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(message.From.Id, UserSettings.FilePath, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>(null));
        var service = new DropboxFileAccessService(settingsPersistenceLayer, Substitute.For<ITelegramBotClientWrapper>());

        // Act
        var result = await service.GetFilePathAsync(message, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }
}
