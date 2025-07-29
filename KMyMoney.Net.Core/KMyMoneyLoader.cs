using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public class KMyMoneyLoader
{
    private readonly IList<IFileAccessor> _fileAccessors;

    internal KMyMoneyLoader(IList<IFileAccessor> fileAccessors)
    {
        _fileAccessors = fileAccessors;
    }

    public Task<KMyMoneyFile> LoadFileAsync(Uri uri)
    {
        var accessor = _fileAccessors.FirstOrDefault(acc => acc.UriSupported(uri)) 
                       ?? throw new($"Don't know what to do with {uri}");
        return LoadFileAsync(accessor, uri);
    }

    public static async Task<KMyMoneyFile> LoadFileAsync(IFileAccessor fileAccessor, Uri uri)
    {
        await using var fileStream = await fileAccessor.GetReadStreamAsync(uri);
        var serializer = new XmlSerializer(typeof(KmyMoneyFileRoot));
        await using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            IgnoreWhitespace = true,
        };

        using var xmlReader = XmlReader.Create(gzipStream, settings);
        var root = (KmyMoneyFileRoot?)serializer.Deserialize(xmlReader) ??
                   throw new($"Could not load KMyMoneyFile");
        return new(uri, fileAccessor, root);
    }
}