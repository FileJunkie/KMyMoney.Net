namespace KMyMoney.Net.TelegramBot.Persistence.InMemory;

public class InMemoryPersistenceLayer : ISettingsPersistenceLayer
{
    private readonly Dictionary<long, string> _tokens = new();
    private readonly Dictionary<long, string> _filePaths = new();

    public Task<string?> GetTokenByUserIdAsync(long userId) =>
        GetToAsync(_tokens, userId);

    public Task SetTokenByUserIdAsync(long userId, string token)
    {
        _tokens[userId] = token;
        return Task.CompletedTask;
    }

    public Task<string?> GetFilePathByUserIdAsync(long userId) =>
        GetToAsync(_filePaths, userId);

    public Task SetFilePathByUserIdAsync(long userId, string filePath)
    {
        _filePaths[userId] = filePath;
        return Task.CompletedTask;
    }

    private static Task<TValue?> GetToAsync<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var value) ?
            Task.FromResult<TValue?>(value) :
            Task.FromResult<TValue?>(default);
    }
}