using System.Diagnostics.CodeAnalysis;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Core.FileAccessors.Dropbox;

namespace KMyMoney.Net.TelegramBot.Dropbox;

[ExcludeFromCodeCoverage(Justification = "Trivial proxy")]
public class DropboxFileAccessorFactory : IFileAccessorFactory
{
    public IFileAccessor CreateFileAccessor(string token) =>
        new DropboxFileAccessor(token);
}