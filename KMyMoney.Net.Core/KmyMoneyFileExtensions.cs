using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public static class KmyMoneyFileExtensions
{
    // TODO: use extension block when .NET 10 is release and C#14 is supported by Rider
    // and make this extension method static 
    // extension(KmyMoneyFile file)
    public static KmyMoneyFile Load(string filePath)
    {
        var serializer = new XmlSerializer(typeof(KmyMoneyFile));
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
        
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            IgnoreWhitespace = true,
        };
        
        using var xmlReader = XmlReader.Create(gzipStream, settings);
        return (KmyMoneyFile?)serializer.Deserialize(xmlReader) ??
               throw new($"Could not load KMyMoneyFile {filePath}");
    }

    public static void Save(this KmyMoneyFile file, string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Create);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
        using var streamWriter = new StreamWriter(gzipStream);
        streamWriter.Write(file.Dump());
    }

    public static string Dump(this KmyMoneyFile file)
    {
        var serializer = new XmlSerializer(typeof(KmyMoneyFile));
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");

        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = " ",
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace,
            OmitXmlDeclaration = true
        };

        using var stringWriter = new StringWriter();
        using (var writer = XmlWriter.Create(stringWriter, settings))
        {
            serializer.Serialize(writer, file, ns);
        }

        var xml = stringWriter.ToString();

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<!DOCTYPE KMYMONEY-FILE>");
        sb.Append(xml);
        return sb.ToString();
    }
}