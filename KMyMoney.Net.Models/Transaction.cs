namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("TRANSACTION")]
public class Transaction
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("postdate")]
    public required string PostDate { get; set; }

    [XmlAttribute("memo")]
    public string? Memo { get; set; }

    [XmlAttribute("entrydate")]
    public required string EntryDate { get; set; }

    [XmlAttribute("commodity")]
    public required string Commodity { get; set; }

    [XmlElement("SPLITS")]
    public required Splits Splits { get; set; }
}
