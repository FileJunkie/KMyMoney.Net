namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class FixVersion
{
    [XmlAttribute("id")]
    public required string Id { get; init; }
}
