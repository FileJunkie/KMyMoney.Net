namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class UserAddress
{
    [XmlAttribute("city")]
    public string City { get; init; } = string.Empty;

    [XmlAttribute("zipcode")]
    public string Zipcode { get; init; } = string.Empty;

    [XmlAttribute("street")]
    public string Street { get; init; } = string.Empty;

    [XmlAttribute("county")]
    public string County { get; init; } = string.Empty;

    [XmlAttribute("telephone")]
    public string Telephone { get; init; } = string.Empty;
}
