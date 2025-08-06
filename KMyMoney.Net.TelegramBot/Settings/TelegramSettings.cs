using System.ComponentModel.DataAnnotations;

namespace KMyMoney.Net.TelegramBot.Settings;

public class TelegramSettings
{
    [Required]
    public required string ApiToken { get; init; }

    public Uri? WebhookUri { get; init; }
}