namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("PAYEE")]
public class Payee
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("name")]
    public required string Name { get; set; }

    [XmlAttribute("matchingenabled")]
    public required string MatchingEnabled { get; set; }

    [XmlAttribute("email")]
    public string? Email { get; set; }

    [XmlAttribute("reference")]
    public string? Reference { get; set; }

    [XmlElement("ADDRESS")]
    public Address? Address { get; set; }
}
