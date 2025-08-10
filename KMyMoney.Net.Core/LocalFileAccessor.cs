using System.Diagnostics.CodeAnalysis;
using KMyMoney.Net.Core.FileAccessors;

namespace KMyMoney.Net.Core;

public class LocalFileAccessor : IFileAccessor
{
    public bool UriSupported(Uri uri) =>
        uri.Scheme == Uri.UriSchemeFile;

    [ExcludeFromCodeCoverage(Justification = "I don't want to create a real file for tests")]
    public Task<Stream> GetReadStreamAsync(Uri uri) =>
        Task.FromResult<Stream>(new FileStream(uri.AbsolutePath, FileMode.Open, FileAccess.Read));

    [ExcludeFromCodeCoverage(Justification = "I don't want to write to a real file in tests")]
    public async Task UpdateFileAsync(Uri uri, Stream stream)
    {
        await using var fileStream = new FileStream(uri.AbsolutePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
    }

    [ExcludeFromCodeCoverage(Justification = "Irrelevant for real-life usecases")]
    public Task<IEnumerable<string>> ListFilesAsync()
    {
        throw new NotImplementedException();
    }
}