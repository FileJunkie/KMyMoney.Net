using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Tests.Common;

public static class TestUtils
{
    public static KmyMoneyFileRoot CreateTestKmyMoneyFileRoot() => new()
    {
        FileInfo = new(),
        User = new()
        {
            Name = "test-user",
            Email = "test@email.com"
        },
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
        Institutions = new() { Values = [] },
        Payees = new() { Values = [] },
        CostCenters = new(),
        Tags = new(),
        Transactions = new() { Values = [] },
        KeyValuePairs = new() { Pair = [] },
        Schedules = new() { Values = [] },
        Securities = new() { Values = [] },
        Currencies = new() { Values = [] },
        Prices = new() { Values = [] },
        Reports = new() { Values = [] },
        Budgets = new(),
        OnlineJobs = new()
    };

    public static MemoryStream CreateCompressedStream(KmyMoneyFileRoot fileRoot)
    {
        var serializer = new XmlSerializer(typeof(KmyMoneyFileRoot));
        var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
        using (var streamWriter = new StreamWriter(gzipStream, Encoding.UTF8))
        {
            serializer.Serialize(streamWriter, fileRoot);
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}