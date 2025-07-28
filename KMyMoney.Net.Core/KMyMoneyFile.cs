using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public class KMyMoneyFile(Uri uri, KmyMoneyFileRoot root)
{
    public Uri Uri { get; } = uri;
    public KmyMoneyFileRoot Root { get; } = root;

    // TODO actually get the stream correctly
    public void Save(Uri uri)
        => Save(uri.AbsolutePath);

    public void Save(string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Create);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
        using var streamWriter = new StreamWriter(gzipStream);
        streamWriter.Write(Dump());
    }

    public string Dump()
    {
        var serializer = new XmlSerializer(typeof(KmyMoneyFileRoot));
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");

        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = " ",
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace,
            OmitXmlDeclaration = true,
        };

        using var stringWriter = new StringWriter();
        using (var writer = XmlWriter.Create(stringWriter, settings))
        {
            serializer.Serialize(writer, Root, ns);
        }

        var xml = stringWriter.ToString();

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<!DOCTYPE KMYMONEY-FILE>");
        sb.Append(xml);
        return sb.ToString();
    }
}