using KMyMoney.Net.Core.FileAccessors;

namespace KMyMoney.Net.Core;

public class LocalFileAccessor : IFileAccessor
{
    public bool UriSupported(Uri uri) =>
        uri.Scheme == Uri.UriSchemeFile;

    public Task<Stream> GetReadStreamAsync(Uri uri) =>
        Task.FromResult<Stream>(new FileStream(uri.AbsolutePath, FileMode.Open, FileAccess.Read));

    public async Task UpdateFileAsync(Uri uri, Stream stream)
    {
        await using var fileStream = new FileStream(uri.AbsolutePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
    }
}