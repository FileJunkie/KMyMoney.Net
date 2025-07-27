using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using KMyMoney.Net.Cli.Options;
using KMyMoney.Net.Core;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Cli.Commands
{
    public static class TestDumpCommands
    {
        public static void Execute(TestDumpOptions opts)
        {
            Console.WriteLine($"Loading file: {opts.FilePath}");
            var kmyMoneyFile = KMyMoneyFileLoader.Load(opts.FilePath);

            if (kmyMoneyFile != null)
            {
                Console.WriteLine("File loaded successfully. Now serializing...");

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

                using (var stringWriter = new StringWriter())
                {
                    using (var writer = XmlWriter.Create(stringWriter, settings))
                    {
                        serializer.Serialize(writer, kmyMoneyFile, ns);
                    }

                    var xml = stringWriter.ToString();

                    xml = xml.Replace("&#xA;", "&#xa;");
                    xml = xml.Replace(" />", "/>");

                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sb.AppendLine("<!DOCTYPE KMYMONEY-FILE>");
                    sb.Append(xml);

                    if (string.IsNullOrEmpty(opts.OutputFile))
                    {
                        Console.Error.WriteLine("Output file not specified.");
                        return;
                    }

                    using (var fileStream = new FileStream(opts.OutputFile, FileMode.Create))
                    {
                        using (var gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
                        {
                            using (var streamWriter = new StreamWriter(gzipStream))
                            {
                                streamWriter.Write(sb.ToString());
                            }
                        }
                    }
                }

                Console.WriteLine($"Successfully serialized to {opts.OutputFile}");
            }
            else
            {
                Console.Error.WriteLine("Failed to load the KMyMoney file.");
            }
        }
    }
}
