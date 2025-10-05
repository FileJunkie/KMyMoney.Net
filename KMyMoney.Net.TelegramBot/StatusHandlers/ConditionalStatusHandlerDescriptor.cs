namespace KMyMoney.Net.TelegramBot.StatusHandlers;

public class ConditionalStatusHandlerDescriptor(IStatusHandler handler, string handledStatus)
{
    public IStatusHandler Handler { get; } = handler;
    public string HandledStatus { get; } = handledStatus;

    public static ConditionalStatusHandlerDescriptor
        FromConditionalStatusHandler<T>(T handler)
        where T : IConditionalStatusHandler =>
        new(handler, T.HandledStatus);
}