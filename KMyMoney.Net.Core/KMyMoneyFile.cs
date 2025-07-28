using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public class KMyMoneyFile(Uri uri, IFileAccessor accessor, KmyMoneyFileRoot root)
{
    public KmyMoneyFileRoot Root { get; } = root;

    public async Task SaveAsync()
    {
        await using var memoryStream = new MemoryStream();

        await using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
        {
            await using var streamWriter = new StreamWriter(gzipStream);
            await streamWriter.WriteAsync(Dump());
        }

        memoryStream.Position = 0;
        await accessor.UpdateFileAsync(uri, memoryStream);
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