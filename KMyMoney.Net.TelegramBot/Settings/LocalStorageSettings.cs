using System.ComponentModel.DataAnnotations;

namespace KMyMoney.Net.TelegramBot.Settings;

public class LocalStorageSettings
{
    [Required]
    public required string FilePath { get; init; }
}