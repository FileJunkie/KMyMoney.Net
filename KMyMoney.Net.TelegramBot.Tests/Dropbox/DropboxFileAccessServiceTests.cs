using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.Core.FileAccessors.Dropbox;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KMyMoney.Net.TelegramBot.Tests.Dropbox;

public class DropboxFileAccessServiceTests
{
    private static IOptions<DropboxSettings> CreateDropboxSettings() =>
        Options.Create(new DropboxSettings
        {
            ApiKey = "key",
            ApiSecret = "secret",
            RedirectUri = "https://redirect"
        });

    [Fact]
    public async Task CreateFileAccessorAsync_WhenRefreshTokenExists_ShouldReturnDropboxFileAccessor()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string refreshToken = "stored_refresh_token";
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From.Id, UserSettings.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(refreshToken);
        var service = new DropboxFileAccessService(
            settingsPersistenceLayer,
            CreateDropboxSettings());

        // Act
        var result = await service.CreateFileAccessorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<DropboxFileAccessor>();
    }

    [Fact]
    public async Task CreateFileAccessorAsync_WhenRefreshTokenIsMissing_ShouldThrowWithUserMessageException()
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

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            Arg.Any<long>(), UserSettings.RefreshToken, Arg.Any<CancellationToken>())
            .Returns((string?)null);

        var service = new DropboxFileAccessService(
            settingsPersistenceLayer,
            CreateDropboxSettings());

        // Act
        var action = () => service.CreateFileAccessorAsync(
            message,
            CancellationToken.None);

        // Assert
        var ex = await action.ShouldThrowAsync<WithUserMessageException>();
        ex.Message.ShouldContain("/login");
    }

    [Fact]
    public async Task GetFilePathAsync_WhenFilePathExists_ShouldReturnFilePath()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string filePath = "/test/file.kmy";
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(message.From.Id, UserSettings.FilePath, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>(filePath));
        var service = new DropboxFileAccessService(
            settingsPersistenceLayer,
            CreateDropboxSettings());

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
        var service = new DropboxFileAccessService(
            settingsPersistenceLayer,
            CreateDropboxSettings());

        // Act
        var action = service.GetFilePathAsync(message, CancellationToken.None);

        // Assert
        await action.ShouldThrowAsync<WithUserMessageException>();
    }
}
