using Dropbox.Api;

namespace KMyMoney.Net.TelegramBot.IntegrationTests;

internal sealed record KMyMoneyIntegrationTestContextOptions(
    bool ThrowUnexpectedFileAccessError = false,
    IReadOnlyList<OAuth2Response?>? OAuth2Responses = null);
