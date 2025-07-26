namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Prices
{
    [XmlAttribute("count")]
    public int Count
    {
        get => PricePair.Length;
        init { }
    }

    [XmlElement("PRICEPAIR")]
    public required PricePair[] PricePair { get; set; } = [];
}
