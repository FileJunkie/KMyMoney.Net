namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class AccountGroup
{
    [XmlAttribute("group")]
    public required string Group { get; init; }
}