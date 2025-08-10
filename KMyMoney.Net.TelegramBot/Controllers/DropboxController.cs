using System.ComponentModel.DataAnnotations;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KMyMoney.Net.TelegramBot.Controllers;

[Route("[controller]")]
[ApiController]
public class DropboxController(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IOptions<DropboxSettings> dropboxSettings,
    IDropboxOAuth2HelperWrapper dropboxOAuth2HelperWrapper,
    ILogger<DropboxController> logger) : ControllerBase
{
    [HttpGet("callback")]
    public async Task<IActionResult> CallbackAsync(
        [FromQuery] [Required] string code,
        [FromQuery] [Required] string state,
        CancellationToken cancellationToken)
    {
        var userId = await settingsPersistenceLayer.GetSavedValueByKeyAsync(
            $"states/{state}",
            cancellationToken: cancellationToken);

        if (userId == null || !long.TryParse(userId, out var userIdLong))
        {
            return BadRequest();
        }

        var token = await dropboxOAuth2HelperWrapper.ProcessCodeFlowAsync(
            code,
            dropboxSettings.Value.ApiKey,
            appSecret: dropboxSettings.Value.ApiSecret,
            redirectUri: dropboxSettings.Value.RedirectUri);

        if (token == null)
        {
            return Forbid();
        }

        logger.LogInformation("Token for user {UserId} saved until {Expiration}", userId, token.ExpiresAt);

        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            userIdLong,
            UserSettings.Token,
            token.AccessToken,
            token.ExpiresAt.HasValue ?
                (token.ExpiresAt.Value - DateTimeOffset.Now) :
                null,
            cancellationToken: cancellationToken);

        return Ok("Logged in");
    }
}