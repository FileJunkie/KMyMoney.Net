using KMyMoney.Net.Core.FileAccessors;

namespace KMyMoney.Net.TelegramBot.IntegrationTests;

internal sealed class TestDropboxFileAccessor : IFileAccessor
{
    // Entirely in-memory virtual Dropbox storage for tests.
    private readonly Dictionary<string, byte[]> _files = new();
    public string UriPrefix => "dropbox://";

    public bool UriSupported(Uri uri) => uri.Scheme == "dropbox";

    public Task<Stream> GetReadStreamAsync(Uri uri)
    {
        var path = NormalizePath(uri.AbsolutePath);
        if (!_files.TryGetValue(path, out var content))
        {
            throw new KeyNotFoundException($"Virtual test file '{path}' not found");
        }

        return Task.FromResult<Stream>(new MemoryStream(content, writable: false));
    }

    public async Task UpdateFileAsync(Uri uri, Stream stream)
    {
        var path = NormalizePath(uri.AbsolutePath);
        await using var copy = new MemoryStream();
        await stream.CopyToAsync(copy);
        _files[path] = copy.ToArray();
    }

    public Task<IEnumerable<string>> ListFilesAsync() =>
        Task.FromResult<IEnumerable<string>>(
            _files.Keys.Where(k => k.EndsWith(".kmy", StringComparison.OrdinalIgnoreCase)));

    public void SeedVirtualFile(string path, Stream compressedStream)
    {
        var normalizedPath = NormalizePath(path);
        using var copy = new MemoryStream();
        compressedStream.CopyTo(copy);
        _files[normalizedPath] = copy.ToArray();
    }

    private static string NormalizePath(string path) => path.StartsWith('/') ? path : $"/{path}";
}
