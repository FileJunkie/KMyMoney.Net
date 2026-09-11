namespace KMyMoney.Net.TelegramBot.Dropbox;

public interface IDropboxTokenManager
{
    Task<string> GetValidAccessTokenAsync(long userId, CancellationToken cancellationToken);
}
