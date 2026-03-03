using KMyMoney.Net.TelegramBot.Persistence;

namespace KMyMoney.Net.TelegramBot.IntegrationTests;

internal sealed class TestSettingsPersistenceLayer : ISettingsPersistenceLayer
{
    private readonly Dictionary<string, Entry> _values = new();
    private DateTimeOffset _now = DateTimeOffset.UtcNow;

    public void AdvanceBy(TimeSpan duration) => _now = _now.Add(duration);

    public Task<string?> GetUserSettingByUserIdAsync(
        long userId,
        UserSettings setting,
        CancellationToken cancellationToken = default) =>
        GetSavedValueByKeyAsync(UserKey(userId, setting), cancellationToken);

    public Task SetUserSettingByUserIdAsync(
        long userId,
        UserSettings setting,
        string? value,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default) =>
        SetSavedValueByKeyAsync(UserKey(userId, setting), value, expiresIn, cancellationToken);

    public Task<string?> GetSavedValueByKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        if (!_values.TryGetValue(key, out var entry))
        {
            return Task.FromResult<string?>(null);
        }

        if (entry.ExpiresAt <= _now)
        {
            _values.Remove(key);
            return Task.FromResult<string?>(null);
        }

        return Task.FromResult<string?>(entry.Value);
    }

    public Task SetSavedValueByKeyAsync(
        string key,
        string? value,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        if (value == null)
        {
            _values.Remove(key);
            return Task.CompletedTask;
        }

        _values[key] = new(value, expiresIn.HasValue ? _now.Add(expiresIn.Value) : DateTimeOffset.MaxValue);
        return Task.CompletedTask;
    }

    private static string UserKey(long userId, UserSettings setting) => $"{userId}_{setting}";

    private readonly record struct Entry(string Value, DateTimeOffset ExpiresAt);
}
