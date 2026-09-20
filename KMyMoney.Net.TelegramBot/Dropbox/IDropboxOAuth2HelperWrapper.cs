using Dropbox.Api;

namespace KMyMoney.Net.TelegramBot.Dropbox;

public interface IDropboxOAuth2HelperWrapper
{
    Uri GetAuthorizeUri(
        OAuthResponseType oauthResponseType,
        string clientId,
        string? redirectUri = null,
        string? state = null,
        bool forceReapprove = false,
        bool disableSignup = false,
        string? requireRole = null,
        bool forceReauthentication = false,
        TokenAccessType tokenAccessType = TokenAccessType.Legacy,
        string[]? scopeList = null,
        IncludeGrantedScopes includeGrantedScopes = IncludeGrantedScopes.None,
        string? codeChallenge = null);

    Task<OAuth2Response?> ProcessCodeFlowAsync(
        string code,
        string appKey,
        string? appSecret = null,
        string? redirectUri = null,
        HttpClient? client = null,
        string? codeVerifier = null);
}