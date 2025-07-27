namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("PAYEE")]
public class Payee : IHasId, IHasName
{
    [XmlAttribute("name")]
    public required string Name { get; init; }

    [XmlAttribute("reference")]
    public string? Reference { get; init; }

    [XmlAttribute("matchingenabled")]
    public required string MatchingEnabled { get; init; }

    [XmlAttribute("id")]
    public required string Id { get; init; }

    [XmlAttribute("email")]
    public string? Email { get; init; }

    [XmlElement("ADDRESS")]
    public PayeeAddress? Address { get; init; }
}
