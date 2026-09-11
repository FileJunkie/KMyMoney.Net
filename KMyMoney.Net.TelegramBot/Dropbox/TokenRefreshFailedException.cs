namespace KMyMoney.Net.TelegramBot.Dropbox;

public class TokenRefreshFailedException : Exception
{
    public TokenRefreshFailedException(string message) : base(message) { }
}
