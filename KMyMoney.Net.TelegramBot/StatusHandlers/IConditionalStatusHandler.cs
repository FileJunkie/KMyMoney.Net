namespace KMyMoney.Net.TelegramBot.StatusHandlers;

public interface IConditionalStatusHandler : IStatusHandler
{
    string HandledStatus { get; }
}