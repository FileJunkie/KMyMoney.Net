namespace KMyMoney.Net.TelegramBot.Persistence.InMemory;

public class InMemoryPersistenceLayer : ISettingsPersistenceLayer
{
    private readonly Dictionary<long, string> _tokens = new();
    private readonly Dictionary<long, string> _filePaths = new();
    private readonly Dictionary<long, string> _statuses = new();

    public Task<string?> GetTokenByUserIdAsync(long userId, CancellationToken cancellationToken) =>
        GetToAsync(_tokens, userId);

    public Task SetTokenByUserIdAsync(long userId, string token, CancellationToken cancellationToken)
    {
        _tokens[userId] = token;
        return Task.CompletedTask;
    }

    public Task<string?> GetFilePathByUserIdAsync(long userId, CancellationToken cancellationToken) =>
        GetToAsync(_filePaths, userId);

    public Task SetFilePathByUserIdAsync(long userId, string filePath, CancellationToken cancellationToken)
    {
        _filePaths[userId] = filePath;
        return Task.CompletedTask;
    }

    public Task<string?> GetStatusByUserIdAsync(long userId, CancellationToken cancellationToken) =>
        GetToAsync(_statuses, userId);

    public Task SetStatusByUserIdAsync(long userId, string? status, CancellationToken cancellationToken)
    {
        if (status == null)
        {
            _statuses.Remove(userId);
        }
        else
        {
            _statuses[userId] = status;
        }
        return Task.CompletedTask;
    }

    private static Task<TValue?> GetToAsync<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
    {
        return dictionary.TryGetValue(key, out var value) ?
            Task.FromResult<TValue?>(value) :
            Task.FromResult<TValue?>(default);
    }
}