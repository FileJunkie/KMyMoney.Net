using System.Diagnostics.CodeAnalysis;

namespace KMyMoney.Net.TelegramBot.Persistence;

public interface ISettingsPersistenceLayer
{
    bool TryGetTokenByUserId(long userId, [MaybeNullWhen(false)] out string token);
    void SetTokenByUserId(long userId, string token);
}