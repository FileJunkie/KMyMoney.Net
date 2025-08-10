using Shouldly;

namespace KMyMoney.Net.Core.Tests;

public class LocalFileAccessorTests
{
    [Theory]
    [InlineData("file:///home/user/test.kmy", true)]
    [InlineData("https://example.com/test.kmy", false)]
    [InlineData("ftp://example.com/test.kmy", false)]
    [InlineData("C:\\Users\\test.kmy", true)]
    public void UriSupported_ShouldReturnCorrectResultForScheme(string uriString, bool expected)
    {
        // Arrange
        var accessor = new LocalFileAccessor();
        var uri = new Uri(uriString);

        // Act
        var result = accessor.UriSupported(uri);

        // Assert
        result.ShouldBe(expected);
    }
}
