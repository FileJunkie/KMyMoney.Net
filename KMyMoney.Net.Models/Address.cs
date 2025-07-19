namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Address
{
    [XmlAttribute("street")]
    public string? Street { get; set; }

    [XmlAttribute("city")]
    public string? City { get; set; }

    [XmlAttribute("county")]
    public string? County { get; set; }

    [XmlAttribute("zipcode")]
    public string? Zipcode { get; set; }

    [XmlAttribute("telephone")]
    public string? Telephone { get; set; }

    [XmlAttribute("postcode")]
    public string? Postcode { get; set; }

    [XmlAttribute("state")]
    public string? State { get; set; }
}
