namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("TRANSACTION")]
public class Transaction : IHasId
{
    [XmlAttribute("id")]
    public required string Id { get; init; }

    [XmlAttribute("postdate")]
    public required string PostDate { get; init; }

    [XmlAttribute("memo")]
    public string Memo { get; init; } = string.Empty;

    [XmlAttribute("entrydate")]
    public required string EntryDate { get; init; }

    [XmlAttribute("commodity")]
    public required string Commodity { get; init; }

    [XmlElement("SPLITS")]
    public required Splits Splits { get; init; }
}
