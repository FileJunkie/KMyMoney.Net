namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class AccountId
{
    [XmlAttribute("id")]
    public required string Id { get; init; }
}
