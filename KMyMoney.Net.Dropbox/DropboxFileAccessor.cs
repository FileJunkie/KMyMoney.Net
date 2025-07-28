using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Stone;

namespace KMyMoney.Net.Dropbox;

public class DropboxFileAccessor
{
    private readonly DropboxClient _client;

    private DropboxFileAccessor()
    {
        _client = null!;
    }

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

    public async Task<IDownloadResponse<FileMetadata>> GetFileAsync(string path)
    {
        return await _client.Files.DownloadAsync(path) ?? throw new FileNotFoundException(path);
    }
}