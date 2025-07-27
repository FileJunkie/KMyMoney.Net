namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("USER")]
public class User : IHasName
{
    [XmlAttribute("name")]
    public required string Name { get; init; }

    [XmlAttribute("email")]
    public required string Email { get; init; }

    [XmlElement("ADDRESS")]
    public UserAddress? Address { get; init; }
}
