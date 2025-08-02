namespace KMyMoney.Net.TelegramBot.Persistence;

public interface ISettingsPersistenceLayer
{
    Task<string?> GetUserSettingByUserIdAsync(long userId, UserSettings setting, CancellationToken cancellationToken);
    Task SetUserSettingByUserIdAsync(long userId, UserSettings setting, string? value, CancellationToken cancellationToken);
}
