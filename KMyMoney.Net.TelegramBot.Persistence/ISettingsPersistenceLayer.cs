namespace KMyMoney.Net.TelegramBot.Persistence;

public interface ISettingsPersistenceLayer
{
    Task<string?> GetTokenByUserIdAsync(long userId, CancellationToken cancellationToken);
    Task SetTokenByUserIdAsync(long userId, string token, CancellationToken cancellationToken);
    Task<string?> GetFilePathByUserIdAsync(long userId, CancellationToken cancellationToken);
    Task SetFilePathByUserIdAsync(long userId, string filePath, CancellationToken cancellationToken);
    Task<string?> GetStatusByUserIdAsync(long userId, CancellationToken cancellationToken);
    Task SetStatusByUserIdAsync(long userId, string? status, CancellationToken cancellationToken);
}