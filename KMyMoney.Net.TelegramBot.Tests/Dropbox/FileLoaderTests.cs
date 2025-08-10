using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.Tests.Common;
using NSubstitute;
using Shouldly;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Dropbox;

public class FileLoaderTests
{
    [Fact]
    public async Task LoadKMyMoneyFile_ShouldReturnFile_WhenTokenAndPathExist()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var fileAccessor = Substitute.For<IFileAccessor>();
        var fileAccessorFactory = Substitute.For<IFileAccessorFactory>();
        var fileLoader = new FileLoader(settingsPersistenceLayer, botWrapper, fileAccessorFactory);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string token = "valid_token";
        const string filePath = "/test.kmy";
        var fileUri = new Uri($"dropbox://{filePath}");

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Token).Returns(token);
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.FilePath).Returns(filePath);
        fileAccessorFactory.CreateFileAccessor(token).Returns(fileAccessor);
        fileAccessor.UriSupported(fileUri).Returns(true);
        fileAccessor.GetReadStreamAsync(fileUri).Returns(TestUtils.CreateCompressedStream(TestUtils.CreateTestKmyMoneyFileRoot()));

        // Act
        var result = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task LoadKMyMoneyFile_ShouldReturnNullAndSendMessage_WhenTokenIsMissing()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var fileAccessorFactory = Substitute.For<IFileAccessorFactory>();
        var fileLoader = new FileLoader(settingsPersistenceLayer, botWrapper, fileAccessorFactory);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Token).Returns((string?)null);

        // Act
        var result = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
        await botClient.Received(1).SendRequest(Arg.Is<SendMessageRequest>(r =>
            r.Text.Contains("Use /login")), CancellationToken.None);
    }

    [Fact]
    public async Task LoadKMyMoneyFile_ShouldReturnNullAndSendMessage_WhenPathIsMissing()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var fileAccessorFactory = Substitute.For<IFileAccessorFactory>();
        var fileLoader = new FileLoader(settingsPersistenceLayer, botWrapper, fileAccessorFactory);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string token = "valid_token";

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Token).Returns(token);
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.FilePath).Returns((string?)null);

        // Act
        var result = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
        await botClient.Received(1).SendRequest(Arg.Is<SendMessageRequest>(r =>
            r.Text.Contains("Use /file")), CancellationToken.None);
    }
}
