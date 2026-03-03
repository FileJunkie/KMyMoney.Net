using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Dropbox;

namespace KMyMoney.Net.TelegramBot.IntegrationTests;

internal sealed class TestDropboxOAuth2HelperWrapper : IDropboxOAuth2HelperWrapper
{
    private readonly Queue<OAuth2Response?> _processCodeFlowResponses;

    public TestDropboxOAuth2HelperWrapper(IEnumerable<OAuth2Response?>? processCodeFlowResponses = null)
    {
        _processCodeFlowResponses = processCodeFlowResponses != null ?
            new Queue<OAuth2Response?>(processCodeFlowResponses) :
            [];
    }

    public Uri GetAuthorizeUri(
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
        string? codeChallenge = null)
    {
        ArgumentNullException.ThrowIfNull(state);
        var escapedState = Uri.EscapeDataString(state);
        return new Uri($"https://dropbox.test/oauth?client_id={clientId}&state={escapedState}");
    }

    public Task<OAuth2Response?> ProcessCodeFlowAsync(
        string code,
        string appKey,
        string? appSecret = null,
        string? redirectUri = null,
        HttpClient? client = null,
        string? codeVerifier = null) =>
        Task.FromResult(_processCodeFlowResponses.Count > 0 ? _processCodeFlowResponses.Dequeue() : null);
}
