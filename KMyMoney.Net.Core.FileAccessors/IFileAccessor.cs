namespace KMyMoney.Net.Core.FileAccessors;

public interface IFileAccessor
{
    bool UriSupported(Uri uri);
    Task<Stream> GetReadStreamAsync(Uri uri);
}