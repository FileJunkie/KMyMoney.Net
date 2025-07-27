namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("INSTITUTION")]
public class Institution : IHasId, IHasName
{
    [XmlAttribute("name")]
    public required string Name { get; init; }

    [XmlAttribute("id")]
    public required string Id { get; init; }

    [XmlAttribute("sortcode")]
    public string? Sortcode { get; init; }

    [XmlAttribute("manager")]
    public string? Manager { get; init; }

    [XmlElement("ADDRESS")]
    public InstitutionAddress? Address { get; init; }

    [XmlArray("ACCOUNTIDS")]
    [XmlArrayItem("ACCOUNTID")]
    public required AccountId[] AccountIds { get; init; } = [];
}
