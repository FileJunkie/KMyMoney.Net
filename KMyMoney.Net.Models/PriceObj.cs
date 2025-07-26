namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class PriceObj
{
    [XmlAttribute("source")]
    public required string Source { get; init; }

    [XmlAttribute("date")]
    public required string Date { get; init; }

    [XmlAttribute("price")]
    public required string Price { get; init; }
}