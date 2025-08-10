using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Tests.Common;
using NSubstitute;
using Shouldly;

namespace KMyMoney.Net.Core.Tests;

public class KMyMoneyLoaderTests
{
    [Fact]
    public async Task LoadFileAsync_ShouldLoadFileFromSupportedAccessor()
    {
        // Arrange
        var uri = new Uri("file:///test.kmy");
        var fileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var compressedStream = TestUtils.CreateCompressedStream(fileRoot);

        var unsupportedAccessor = Substitute.For<IFileAccessor>();
        unsupportedAccessor.UriSupported(uri).Returns(false);

        var supportedAccessor = Substitute.For<IFileAccessor>();
        supportedAccessor.UriSupported(uri).Returns(true);
        supportedAccessor.GetReadStreamAsync(uri).Returns(compressedStream);

        var loader = new KMyMoneyLoaderBuilder()
            .WithFileAccessor(unsupportedAccessor)
            .WithFileAccessor(supportedAccessor)
            .Build();

        // Act
        var kmyFile = await loader.LoadFileAsync(uri);

        // Assert
        kmyFile.ShouldNotBeNull();
        kmyFile.Root.ShouldNotBeNull();
        kmyFile.Root.User.Name.ShouldBe(fileRoot.User.Name);
        await supportedAccessor.Received(1).GetReadStreamAsync(uri);
        await unsupportedAccessor.DidNotReceive().GetReadStreamAsync(uri);
    }

    [Fact]
    public async Task LoadFileAsync_ShouldThrow_WhenNoAccessorSupportsUri()
    {
        // Arrange
        var uri = new Uri("unsupported:///test.kmy");
        var accessor = Substitute.For<IFileAccessor>();
        accessor.UriSupported(uri).Returns(false);
        var loader = new KMyMoneyLoaderBuilder()
            .WithFileAccessor(accessor)
            .Build();

        // Act & Assert
        var exception = await Should.ThrowAsync<Exception>(() => loader.LoadFileAsync(uri));
        exception.Message.ShouldBe($"Don't know what to do with {uri}");
    }

    [Fact]
    public async Task LoadFileAsync_WithAccessor_ShouldDeserializeFileCorrectly()
    {
        // Arrange
        var uri = new Uri("file:///test.kmy");
        var fileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var compressedStream = TestUtils.CreateCompressedStream(fileRoot);
        var accessor = Substitute.For<IFileAccessor>();
        accessor.GetReadStreamAsync(uri).Returns(compressedStream);

        // Act
        var kmyFile = await KMyMoneyLoader.LoadFileAsync(accessor, uri);

        // Assert
        kmyFile.ShouldNotBeNull();
        kmyFile.Root.ShouldBeEquivalentTo(fileRoot);
    }
}