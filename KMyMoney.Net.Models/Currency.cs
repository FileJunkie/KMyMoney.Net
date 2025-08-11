namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Currency : IHasId, IHasName
{
    [XmlAttribute("scf")]
    public required string Scf { get; init; }

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

    [XmlAttribute("pp")]
    public required string Pp { get; init; }

    [XmlAttribute("symbol")]
    public required string Symbol { get; init; }
}
