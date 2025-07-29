using System.ComponentModel.DataAnnotations;

namespace KMyMoney.Net.TelegramBot.Settings;

public class DropboxSettings
{
    [Required]
    public required string ApiKey { get; init; }
    [Required]
    public required string ApiSecret { get; init; }
}