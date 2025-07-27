namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("SECURITY")]
public class Security : IHasId, IHasName
{
    [XmlAttribute("name")]
    public required string Name { get; init; }

    [XmlAttribute("type")]
    public required string Type { get; init; }

    [XmlAttribute("rounding-method")]
    public required string RoundingMethod { get; init; }

    [XmlAttribute("id")]
    public required string Id { get; init; }

    [XmlAttribute("saf")]
    public required string Saf { get; init; }

    [XmlAttribute("trading-market")]
    public required string TradingMarket { get; init; }

    [XmlAttribute("pp")]
    public required string Pp { get; init; }

    [XmlAttribute("symbol")]
    public required string Symbol { get; init; }

    [XmlAttribute("trading-currency")]
    public required string TradingCurrency { get; init; }

    [XmlElement("KEYVALUEPAIRS")]
    public KeyValuePairs? KeyValuePairs { get; init; }
}
