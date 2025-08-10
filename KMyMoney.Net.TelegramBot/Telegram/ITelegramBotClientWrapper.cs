using Telegram.Bot;

namespace KMyMoney.Net.TelegramBot.Telegram;

public interface ITelegramBotClientWrapper
{
    ITelegramBotClient Bot { get; }
    Task StopAsync();
}