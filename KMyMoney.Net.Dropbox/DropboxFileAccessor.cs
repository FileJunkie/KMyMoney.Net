using Dropbox.Api;
using Dropbox.Api.Files;
using KMyMoney.Net.Core.FileAccessors;

namespace KMyMoney.Net.Dropbox;

public class DropboxFileAccessor : IFileAccessor
{
    private readonly DropboxClient _client;

    private DropboxFileAccessor(string token)
    {
        _client = new(token);
    }
    
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
        var code = await codeRequester(uri.ToString());
        var token = await DropboxOAuth2Helper.ProcessCodeFlowAsync(code, apiKey, apiSecret);
        return new (token.AccessToken);
    }

    public bool UriSupported(Uri uri) =>
        uri.Scheme == "dropbox";

    public async Task<Stream> GetReadStreamAsync(Uri uri)
    {
        var file = await _client.Files.DownloadAsync(uri.AbsolutePath);
        return await file.GetContentAsStreamAsync();
    }

    public async Task UpdateFileAsync(Uri uri, Stream stream)
    {
        await _client.Files.UploadAsync(
            path: uri.AbsolutePath,
            mode: WriteMode.Overwrite.Instance,
            body: stream);
    }
}