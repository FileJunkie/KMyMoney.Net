using System.ComponentModel.DataAnnotations;

namespace KMyMoney.Net.Core.FileAccessors.Dropbox;

public class DropboxSettings
{
    [Required]
    public required string ApiKey { get; init; }
    [Required]
    public required string ApiSecret { get; init; }
    public string? RedirectUri { get; init; }
}
