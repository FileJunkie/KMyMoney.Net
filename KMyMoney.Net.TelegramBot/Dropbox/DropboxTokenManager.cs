using System.Diagnostics.CodeAnalysis;
using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;

namespace KMyMoney.Net.TelegramBot.Dropbox;

[ExcludeFromCodeCoverage(Justification = "Integrates with real Dropbox API")]
public class DropboxTokenManager(
    ISettingsPersistenceLayer? settingsPersistenceLayer = null,
    IDropboxOAuth2HelperWrapper? dropboxOAuth2HelperWrapper = null,
    IOptions<DropboxSettings>? dropboxSettings = null,
    ILogger<DropboxTokenManager>? logger = null) : IDropboxTokenManager
{
    private readonly DropboxSettings _dropboxSettings = dropboxSettings?.Value ?? throw new ArgumentNullException(nameof(dropboxSettings));
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer ?? throw new ArgumentNullException(nameof(settingsPersistenceLayer));
    private readonly IDropboxOAuth2HelperWrapper _dropboxOAuth2HelperWrapper = dropboxOAuth2HelperWrapper ?? throw new ArgumentNullException(nameof(dropboxOAuth2HelperWrapper));
    private readonly ILogger<DropboxTokenManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<string> GetValidAccessTokenAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        var accessToken = await _settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            userId, UserSettings.Token, cancellationToken);

        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogInformation("No access token for user {UserId}, forcing re-login", userId);
            throw new TokenRefreshFailedException("No access token. Please use /login");
        }

        if (await IsTokenValidAsync(accessToken, cancellationToken))
        {
            return accessToken;
        }

        _logger.LogInformation("Access token invalid for user {UserId}, attempting refresh", userId);

        try
        {
            var refreshedToken = await RefreshAccessTokenAsync(userId, cancellationToken);
            return refreshedToken;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to refresh token for user {UserId}", userId);
            throw new TokenRefreshFailedException(
                "Session expired. Please use /login to re-authorize Dropbox access.");
        }
    }

    private async Task<bool> IsTokenValidAsync(
        string accessToken,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = new DropboxClient(accessToken);
            await client.Users.GetCurrentAccountAsync();
            return true;
        }
        catch (Exception ex) when (IsTokenRelatedError(ex))
        {
            _logger.LogDebug(ex, "Token validation failed");
            return false;
        }
    }

    private static bool IsTokenRelatedError(Exception ex)
    {
        return ex.Message.Contains("401")
            || ex.Message.Contains("Unauthorized")
            || ex.Message.Contains("invalid_token")
            || ex.Message.Contains("access denied")
            || ex.Message.Contains("invalid_access_token");
    }

    private async Task<string> RefreshAccessTokenAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        var refreshToken = await _settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            userId, UserSettings.RefreshToken, cancellationToken);

        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("No refresh token for user {UserId}", userId);
            throw new InvalidOperationException("No refresh token available");
        }

        _logger.LogInformation("Refreshing access token for user {UserId}", userId);

        var token = await _dropboxOAuth2HelperWrapper.ProcessRefreshFlowAsync(
            refreshToken,
            _dropboxSettings.ApiKey,
            _dropboxSettings.ApiSecret,
            _dropboxSettings.RedirectUri);

        if (token?.AccessToken == null)
        {
            _logger.LogError("Refresh token flow returned null for user {UserId}", userId);
            throw new InvalidOperationException("Token refresh failed");
        }

        await _settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            userId,
            UserSettings.Token,
            token.AccessToken,
            token.ExpiresAt.HasValue ? (token.ExpiresAt.Value - DateTimeOffset.Now) : null,
            cancellationToken);

        _logger.LogInformation("Access token refreshed for user {UserId}, expires at {ExpiresAt}",
            userId, token.ExpiresAt);

        return token.AccessToken;
    }
}
