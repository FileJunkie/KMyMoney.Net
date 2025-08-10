using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KMyMoney.Net.TelegramBot.Controllers;

[Route("[controller]")]
[ApiController]
public class WebhookController(
    IUpdateHandler updateHandler,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    ILogger<WebhookController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string telegramToken,
        CancellationToken cancellationToken)
    {
        if (telegramToken !=
            await settingsPersistenceLayer.GetSavedValueByKeyAsync("telegramSecret", cancellationToken))
        {
            return Forbid();
        }

        try
        {
            if (update.Message != null)
            {
                await updateHandler.OnMessageAsync(update.Message, UpdateType.Message, cancellationToken);
            }
            else
            {
                logger.LogTrace("UpdateType ignored");
            }
        }
        catch (Exception exception)
        {
            await updateHandler.OnErrorAsync(exception, global::Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError);
        }

        return Ok();
    }
}