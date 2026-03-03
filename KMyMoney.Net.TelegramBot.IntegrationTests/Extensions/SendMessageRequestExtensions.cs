using Shouldly;

namespace KMyMoney.Net.TelegramBot.IntegrationTests.Extensions;

internal static class SendMessageRequestExtensions
{
    public static string GetRequiredText(this global::Telegram.Bot.Requests.SendMessageRequest request)
    {
        request.Text.ShouldNotBeNull();
        return request.Text;
    }
}
