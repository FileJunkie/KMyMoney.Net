namespace KMyMoney.Net.TelegramBot.Persistence.InMemory;

public class InMemoryPersistenceLayer : ISettingsPersistenceLayer
{
    private readonly Dictionary<string, string> _settings = new();

    public Task<string?> GetUserSettingByUserIdAsync(
        long userId,
        UserSettings setting,
        CancellationToken cancellationToken = default) =>
        GetSavedValueByKeyAsync($"{userId}_{setting}", cancellationToken);

    public Task SetUserSettingByUserIdAsync(
        long userId,
        UserSettings setting,
        string? value,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default) =>
        SetSavedValueByKeyAsync($"{userId}_{setting}", value, expiresIn, cancellationToken);

    public Task<string?> GetSavedValueByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return _settings.TryGetValue(key, out var value)
            ? Task.FromResult<string?>(value)
            : Task.FromResult<string?>(null);
    }

    public Task SetSavedValueByKeyAsync(string key, string? value, TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        if (value == null)
        {
            _settings.Remove(key);
        }
        else
        {
            _settings[key] = value;
        }

        return Task.CompletedTask;
    }
}
