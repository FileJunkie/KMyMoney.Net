namespace KMyMoney.Net.TelegramBot.Persistence.InMemory;

public class InMemoryPersistenceLayer : ISettingsPersistenceLayer
{
    private readonly Dictionary<(long, UserSettings), string> _settings = new();

    public Task<string?> GetUserSettingByUserIdAsync(long userId, UserSettings setting, CancellationToken cancellationToken)
    {
        return _settings.TryGetValue((userId, setting), out var value)
            ? Task.FromResult<string?>(value)
            : Task.FromResult<string?>(null);
    }

    public Task SetUserSettingByUserIdAsync(long userId, UserSettings setting, string? value, CancellationToken cancellationToken)
    {
        if (value == null)
        {
            _settings.Remove((userId, setting));
        }
        else
        {
            _settings[(userId, setting)] = value;
        }

        return Task.CompletedTask;
    }
}
