namespace KMyMoney.Net.TelegramBot.Exceptions;

public class WithUserMessageException(string message, bool keepStatus = false) : Exception(message)
{
    public bool KeepStatus => keepStatus;
}