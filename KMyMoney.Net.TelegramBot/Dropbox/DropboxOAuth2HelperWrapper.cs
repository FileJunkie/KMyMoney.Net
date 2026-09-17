using System.Diagnostics.CodeAnalysis;
using Dropbox.Api;

namespace KMyMoney.Net.TelegramBot.Dropbox;

[ExcludeFromCodeCoverage(Justification = "It's a thin wrapper")]
public class DropboxOAuth2HelperWrapper : IDropboxOAuth2HelperWrapper
{
    public Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, string? redirectUri = null,
        string? state = null, bool forceReapprove = false, bool disableSignup = false, string? requireRole = null,
        bool forceReauthentication = false, TokenAccessType tokenAccessType = TokenAccessType.Legacy,
        string[]? scopeList = null, IncludeGrantedScopes includeGrantedScopes = IncludeGrantedScopes.None,
        string? codeChallenge = null) =>
        DropboxOAuth2Helper.GetAuthorizeUri(
            oauthResponseType: oauthResponseType,
            clientId: clientId,
            redirectUri: redirectUri,
            state: state,
            forceReapprove: forceReapprove,
            disableSignup: disableSignup,
            requireRole: requireRole,
            forceReauthentication: forceReauthentication,
            tokenAccessType: tokenAccessType,
            scopeList: scopeList,
            includeGrantedScopes: includeGrantedScopes,
            codeChallenge: codeChallenge);

    public Task<OAuth2Response?> ProcessCodeFlowAsync(string code, string appKey, string? appSecret = null,
        string? redirectUri = null, HttpClient? client = null, string? codeVerifier = null) =>
        DropboxOAuth2Helper.ProcessCodeFlowAsync(
            code: code,
            appKey: appKey,
            appSecret: appSecret,
            redirectUri: redirectUri,
            client: client,
            codeVerifier: codeVerifier);
}