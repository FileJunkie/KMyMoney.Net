using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Models;
using NSubstitute;
using Shouldly;

namespace KMyMoney.Net.Core.Tests;

public class KMyMoneyFileTests
{
    [Fact]
    public void Dump_ShouldProduceDeserializableXml()
    {
        // Arrange
        var fileRoot = CreateTestKmyMoneyFileRoot();
        var fileAccessor = Substitute.For<IFileAccessor>();
        var uri = new Uri("file:///test.kmy");
        var kmyFile = new KMyMoneyFile(uri, fileAccessor, fileRoot);

        // Act
        var xmlDump = kmyFile.Dump();

        // Assert
        xmlDump.ShouldStartWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xmlDump.ShouldContain("<!DOCTYPE KMYMONEY-FILE>");

        var deserializedRoot = DeserializeKmyMoneyFileRoot(xmlDump);
        deserializedRoot.ShouldBeEquivalentTo(fileRoot);
    }

    [Fact]
    public async Task SaveAsync_ShouldProduceDeserializableCompressedData()
    {
        // Arrange
        var fileRoot = CreateTestKmyMoneyFileRoot();
        var fileAccessor = Substitute.For<IFileAccessor>();
        var uri = new Uri("file:///test.kmy");
        var kmyFile = new KMyMoneyFile(uri, fileAccessor, fileRoot);
        MemoryStream? savedStream = null;

        await fileAccessor.UpdateFileAsync(uri, Arg.Do<Stream>(s =>
        {
            savedStream = new MemoryStream();
            s.CopyTo(savedStream);
            savedStream.Position = 0;
        }));

        // Act
        await kmyFile.SaveAsync();

        // Assert
        await fileAccessor.Received(1).UpdateFileAsync(uri, Arg.Any<Stream>());
        savedStream.ShouldNotBeNull();

        try
        {
            savedStream.Length.ShouldBeGreaterThan(0);

            await using var gzipStream = new GZipStream(savedStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(gzipStream, Encoding.UTF8);
            var decompressedContent = await streamReader.ReadToEndAsync();

            decompressedContent.ShouldStartWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            decompressedContent.ShouldContain("<!DOCTYPE KMYMONEY-FILE>");

            var deserializedRoot = DeserializeKmyMoneyFileRoot(decompressedContent);
            deserializedRoot.ShouldBeEquivalentTo(fileRoot);
        }
        finally
        {
            if (savedStream != null)
            {
                await savedStream.DisposeAsync();
            }
        }
    }

    private static KmyMoneyFileRoot CreateTestKmyMoneyFileRoot() => new()
    {
        FileInfo = new(),
        User = new()
        {
            Name = "test-user",
            Email = "test@email.com"
        },
        Institutions = new() { Values = [] },
        Payees = new() { Values = [] },
        CostCenters = new(),
        Tags = new(),
        Accounts = new()
        {
            Values =
            [
                new()
                {
                    Id = "A000001",
                    Name = "Checking Account",
                    Type = "Asset",
                    Currency = "USD"
                }
            ]
        },
        Transactions = new()
        {
            Values =
            [
                new()
                {
                    Id = "T000001",
                    Commodity = "USD",
                    PostDate = "2025-01-15",
                    EntryDate = "2025-01-15",
                    Memo = "Test Transaction",
                    Splits = new()
                    {
                        Split =
                        [
                            new()
                            {
                                Id = "S001",
                                Account = "A000001",
                                Value = "100/1",
                                Shares = "100/1",
                                Memo = "Split Memo"
                            }
                        ]
                    }
                }
            ]
        },
        KeyValuePairs = new() { Pair = [] },
        Schedules = new() { Values = [] },
        Securities = new() { Values = [] },
        Currencies = new() { Values = [] },
        Prices = new() { Values = [] },
        Reports = new() { Values = [] },
        Budgets = new(),
        OnlineJobs = new()
    };

    private static KmyMoneyFileRoot DeserializeKmyMoneyFileRoot(string xml)
    {
        var serializer = new XmlSerializer(typeof(KmyMoneyFileRoot));
        using var stringReader = new StringReader(xml);
        return (KmyMoneyFileRoot)serializer.Deserialize(stringReader)!;
    }
}
