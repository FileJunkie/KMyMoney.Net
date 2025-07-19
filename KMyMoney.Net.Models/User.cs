namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("USER")]
public class User
{
    [XmlAttribute("name")]
    public required string Name { get; set; }

    [XmlAttribute("email")]
    public required string Email { get; set; }

    [XmlElement("ADDRESS")]
    public Address? Address { get; set; }
}
