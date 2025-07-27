namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class PricePair
{
    [XmlAttribute("to")]
    public required string To { get; init; }

    [XmlAttribute("from")]
    public required string From { get; init; }

    [XmlElement("PRICE")]
    public required PriceObj[] Price { get; init; } = [];
}