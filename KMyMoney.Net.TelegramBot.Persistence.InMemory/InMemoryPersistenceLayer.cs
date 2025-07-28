using System.Diagnostics.CodeAnalysis;

namespace KMyMoney.Net.TelegramBot.Persistence.InMemory;

public class InMemoryPersistenceLayer : ISettingsPersistenceLayer
{
    private readonly IDictionary<long, string> _settings = new Dictionary<long, string>();

    public bool TryGetTokenByUserId(long userId, [MaybeNullWhen(false)] out string token)
        => _settings.TryGetValue(userId, out token);

    public void SetTokenByUserId(long userId, string token)
        => _settings[userId] = token;
}