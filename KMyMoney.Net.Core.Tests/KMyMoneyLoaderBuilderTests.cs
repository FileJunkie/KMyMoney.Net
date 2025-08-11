using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Tests.Common;
using NSubstitute;
using Shouldly;

namespace KMyMoney.Net.Core.Tests;

public class KMyMoneyLoaderBuilderTests
{
    [Fact]
    public void Build_ShouldReturnNonNullLoader()
    {
        // Arrange
        var builder = new KMyMoneyLoaderBuilder();

        // Act
        var loader = builder.Build();

        // Assert
        loader.ShouldNotBeNull();
    }

    [Fact]
    public async Task WithFileAccessor_ShouldAddAccessorToLoader()
    {
        // Arrange
        var testKmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();

        var uri = new Uri("file:///test.kmy");
        var accessor = Substitute.For<IFileAccessor>();
        accessor.UriSupported(uri).Returns(true);
        accessor.GetReadStreamAsync(uri).Returns(TestUtils.CreateCompressedStream(testKmyMoneyFileRoot));

        var builder = new KMyMoneyLoaderBuilder();

        // Act
        var loader = builder
            .WithFileAccessor(accessor)
            .Build();

        var kmyFile = await loader.LoadFileAsync(uri);

        // Assert
        kmyFile.ShouldNotBeNull();
        await accessor.Received(1).GetReadStreamAsync(uri);
    }

    [Fact]
    public async Task WithFileAccessor_ShouldAllowChaining()
    {
        // Arrange
        var testKmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();

        var uri1 = new Uri("file:///test1.kmy");
        var accessor1 = Substitute.For<IFileAccessor>();
        accessor1.UriSupported(uri1).Returns(true);
        accessor1.GetReadStreamAsync(uri1).Returns(TestUtils.CreateCompressedStream(testKmyMoneyFileRoot));

        var uri2 = new Uri("file:///test2.kmy");
        var accessor2 = Substitute.For<IFileAccessor>();
        accessor2.UriSupported(uri2).Returns(true);
        accessor2.GetReadStreamAsync(uri2).Returns(TestUtils.CreateCompressedStream(testKmyMoneyFileRoot));

        var builder = new KMyMoneyLoaderBuilder();

        // Act
        var loader = builder
            .WithFileAccessor(accessor1)
            .WithFileAccessor(accessor2)
            .Build();

        await loader.LoadFileAsync(uri1);
        await loader.LoadFileAsync(uri2);

        // Assert
        await accessor1.Received(1).GetReadStreamAsync(uri1);
        await accessor2.Received(1).GetReadStreamAsync(uri2);
    }
}
