namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("INSTITUTION")]
public class Institution
{
    [XmlAttribute("manager")]
    public string? Manager { get; set; }

    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("name")]
    public required string Name { get; set; }

    [XmlAttribute("sortcode")]
    public string? Sortcode { get; set; }

    [XmlElement("ADDRESS")]
    public Address? Address { get; set; }

    [XmlArray("ACCOUNTIDS")]
    [XmlArrayItem("ACCOUNTID")]
    public required AccountId[] AccountIds { get; set; } = [];
}
