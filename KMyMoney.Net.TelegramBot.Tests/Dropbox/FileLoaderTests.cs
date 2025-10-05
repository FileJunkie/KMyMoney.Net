using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.Tests.Common;
using NSubstitute;
using Shouldly;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Dropbox;

public class FileLoaderTests
{
    [Fact]
    public async Task LoadKMyMoneyFileOrSendErrorAsync_ShouldReturnFile_WhenFileAccessorAndPathExist()
    {
        // Arrange
        var fileAccessService = Substitute.For<IFileAccessService>();
        var fileLoader = new FileLoader(fileAccessService);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        const string filePath = "/test.kmy";
        var fileUri = new Uri($"test-scheme://{filePath}");
        var fileAccessor = Substitute.For<IFileAccessor>();

        fileAccessService.CreateFileAccessorAsync(message, CancellationToken.None).Returns(fileAccessor);
        fileAccessService.GetFilePathAsync(message, CancellationToken.None).Returns(filePath);
        
        fileAccessor.UriPrefix.Returns("test-scheme://");
        fileAccessor.UriSupported(fileUri).Returns(true);
        fileAccessor.GetReadStreamAsync(fileUri).Returns(TestUtils.CreateCompressedStream(TestUtils.CreateTestKmyMoneyFileRoot()));

        // Act
        var result = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task LoadKMyMoneyFileOrSendErrorAsync_ShouldReturnNull_WhenFileAccessorIsNull()
    {
        // Arrange
        var fileAccessService = Substitute.For<IFileAccessService>();
        var fileLoader = new FileLoader(fileAccessService);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };

        fileAccessService.CreateFileAccessorAsync(message, CancellationToken.None).Returns((IFileAccessor?)null);

        // Act
        var result = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task LoadKMyMoneyFileOrSendErrorAsync_ShouldReturnNull_WhenFilePathIsNull()
    {
        // Arrange
        var fileAccessService = Substitute.For<IFileAccessService>();
        var fileLoader = new FileLoader(fileAccessService);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        var fileAccessor = Substitute.For<IFileAccessor>();

        fileAccessService.CreateFileAccessorAsync(message, CancellationToken.None).Returns(fileAccessor);
        fileAccessService.GetFilePathAsync(message, CancellationToken.None).Returns((string?)null);

        // Act
        var result = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }
    
    [Fact]
    public async Task LoadKMyMoneyFileOrSendErrorAsync_ShouldReturnNull_WhenFilePathIsEmpty()
    {
        // Arrange
        var fileAccessService = Substitute.For<IFileAccessService>();
        var fileLoader = new FileLoader(fileAccessService);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        var fileAccessor = Substitute.For<IFileAccessor>();

        fileAccessService.CreateFileAccessorAsync(message, CancellationToken.None).Returns(fileAccessor);
        fileAccessService.GetFilePathAsync(message, CancellationToken.None).Returns(string.Empty);

        // Act
        var result = await fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }
}
