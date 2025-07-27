namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Version
{
    [XmlAttribute("id")]
    public required string Id { get; init; }
}
