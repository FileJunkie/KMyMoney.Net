using System.Diagnostics.CodeAnalysis;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace KMyMoney.Net.Core.FileAccessors.Dropbox;

public class DropboxFileAccessor(string token) : IFileAccessor
{
    private readonly DropboxClient _client = new(token);

    [ExcludeFromCodeCoverage(Justification = "Makes a call to the real API")]
    public static async Task<DropboxFileAccessor> CreateAsync(
        string apiKey,
        string apiSecret,
        CodeRequester codeRequester)
    {
        var uri = DropboxOAuth2Helper.GetAuthorizeUri(
            OAuthResponseType.Code,
            clientId: apiKey,
            tokenAccessType: TokenAccessType.Legacy,
            redirectUri: (string?)null);
        var code = await codeRequester(uri);
        var token = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code, apiKey, apiSecret);
        return new (token.AccessToken);
    }

    public bool UriSupported(Uri uri) =>
        uri.Scheme == "dropbox";

    [ExcludeFromCodeCoverage(Justification = "Requires mocking too much code")]
    public async Task<Stream> GetReadStreamAsync(Uri uri)
    {
        var file = await _client.Files.DownloadAsync(uri.AbsolutePath);
        return await file.GetContentAsStreamAsync();
    }

    [ExcludeFromCodeCoverage(Justification = "Requires mocking too much code")]
    public async Task UpdateFileAsync(Uri uri, Stream stream)
    {
        await _client.Files.UploadAsync(
            path: uri.AbsolutePath,
            mode: WriteMode.Overwrite.Instance,
            body: stream);
    }

    [ExcludeFromCodeCoverage(Justification = "Requires mocking too much code")]
    public async Task<IEnumerable<string>> ListFilesAsync()
    {
        var searchResults = await _client.Files.SearchV2Async(
            new("*.kmy", options:
                new(fileExtensions: ["kmy"])));
        return searchResults.Matches.Select(m => m.Metadata.AsMetadata.Value.PathLower);
    }
}