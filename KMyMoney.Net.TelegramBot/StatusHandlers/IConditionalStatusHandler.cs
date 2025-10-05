namespace KMyMoney.Net.TelegramBot.StatusHandlers;

public interface IConditionalStatusHandler : IStatusHandler
{
    static abstract string HandledStatus { get; }
}