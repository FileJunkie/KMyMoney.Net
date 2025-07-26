namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("SECURITY")]
public class Security
{
    [XmlAttribute("name")]
    public required string Name { get; set; }

    [XmlAttribute("type")]
    public required string Type { get; set; }

    [XmlAttribute("rounding-method")]
    public required string RoundingMethod { get; set; }

    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("saf")]
    public required string Saf { get; set; }

    [XmlAttribute("trading-market")]
    public required string TradingMarket { get; set; }

    [XmlAttribute("pp")]
    public required string Pp { get; set; }

    [XmlAttribute("symbol")]
    public required string Symbol { get; set; }

    [XmlAttribute("trading-currency")]
    public required string TradingCurrency { get; set; }

    [XmlElement("KEYVALUEPAIRS")]
    public KeyValuePairs? KeyValuePairs { get; set; }
}
