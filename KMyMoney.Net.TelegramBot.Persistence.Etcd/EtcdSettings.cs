using System.ComponentModel.DataAnnotations;

namespace KMyMoney.Net.TelegramBot.Persistence.Etcd;

public class EtcdSettings
{
    [Required]
    public required string ConnectionString { get; init; }
    [Required]
    public required string RootCertificate { get; init; }
    [Required]
    public required string Certificate { get; init; }
    [Required]
    public required string Key { get; init; }

    public string KeyPrefix { get; init; } = "/kmymoney.net";
}