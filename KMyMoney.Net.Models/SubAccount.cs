namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class SubAccount
{
    [XmlAttribute("id")]
    public required string Id { get; init; }
}
