namespace KMyMoney.Net.TelegramBot.Persistence;

public interface ISettingsPersistenceLayer
{
    Task<string?> GetTokenByUserIdAsync(long userId);
    Task SetTokenByUserIdAsync(long userId, string token);
    Task<string?> GetFilePathByUserIdAsync(long userId);
    Task SetFilePathByUserIdAsync(long userId, string filePath);
}