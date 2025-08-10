using KMyMoney.Net.Core.FileAccessors;

namespace KMyMoney.Net.TelegramBot.Dropbox;

public interface IFileAccessorFactory
{
    IFileAccessor CreateFileAccessor(string token);
}