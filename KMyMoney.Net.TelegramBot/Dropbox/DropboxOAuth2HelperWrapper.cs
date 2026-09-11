using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
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

    public async Task<OAuth2Response?> ProcessRefreshFlowAsync(
        string refreshToken,
        string appKey,
        string appSecret,
        string? redirectUri = null,
        HttpClient? client = null)
    {
        using var httpClient = client ?? new HttpClient();
        var request = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = appKey,
            ["client_secret"] = appSecret
        };

        var response = await httpClient.PostAsync(
            "https://api.dropboxapi.com/oauth2/token",
            new FormUrlEncodedContent(request));

        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OAuth2Response>(responseString);
    }
}