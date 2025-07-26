namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class UserAddress
{
    [XmlAttribute("city")]
    public string City { get; set; } = string.Empty;

    [XmlAttribute("zipcode")]
    public string Zipcode { get; set; } = string.Empty;

    [XmlAttribute("street")]
    public string Street { get; set; } = string.Empty;

    [XmlAttribute("county")]
    public string County { get; set; } = string.Empty;

    [XmlAttribute("telephone")]
    public string Telephone { get; set; } = string.Empty;
}
