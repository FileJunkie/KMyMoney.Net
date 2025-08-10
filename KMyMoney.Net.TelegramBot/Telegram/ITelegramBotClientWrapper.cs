using Telegram.Bot;

namespace KMyMoney.Net.TelegramBot.Telegram;

public interface ITelegramBotClientWrapper
{
    TelegramBotClient Bot { get; }
    Task StopAsync();
}