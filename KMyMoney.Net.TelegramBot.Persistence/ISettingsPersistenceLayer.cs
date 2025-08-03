namespace KMyMoney.Net.TelegramBot.Persistence;

public interface ISettingsPersistenceLayer
{
    Task<string?> GetUserSettingByUserIdAsync(
        long userId,
        UserSettings setting,
        CancellationToken cancellationToken = default);
    Task SetUserSettingByUserIdAsync(
        long userId,
        UserSettings setting,
        string? value,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default);
    Task<string?> GetSavedValueByKeyAsync(
        string key,
        CancellationToken cancellationToken = default);
    Task SetSavedValueByKeyAsync(
        string key,
        string? value,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default);
}
