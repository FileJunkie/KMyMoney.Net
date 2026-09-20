using Shouldly;

namespace KMyMoney.Net.Core.FileAccessors.Dropbox.Tests;

public class DropboxFileAccessorTests
{
    [Theory]
    [InlineData("dropbox:///test.kmy", true)]
    [InlineData("file:///home/user/test.kmy", false)]
    [InlineData("https://example.com/test.kmy", false)]
    public void UriSupported_ShouldReturnCorrectResultForScheme(string uriString, bool expected)
    {
        // Arrange
        var accessor = new DropboxFileAccessor("dummy_refresh_token", new DropboxSettings { ApiKey = "key", ApiSecret = "secret" });
        var uri = new Uri(uriString);

        // Act
        var result = accessor.UriSupported(uri);

        // Assert
        result.ShouldBe(expected);
    }
}
