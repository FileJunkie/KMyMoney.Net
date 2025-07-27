using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public static class KMyMoneyFileLoader
{
    public static KmyMoneyFile? Load(string filePath)
    {
        var serializer = new XmlSerializer(typeof(KmyMoneyFile));
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
        
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            IgnoreWhitespace = true
        };

        using var xmlReader = XmlReader.Create(gzipStream, settings);
        return (KmyMoneyFile?)serializer.Deserialize(xmlReader);
    }
}
