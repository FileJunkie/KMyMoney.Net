namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class InstitutionAddress
{
    [XmlAttribute("city")]
    public string City { get; set; } = string.Empty;

    [XmlAttribute("street")]
    public string Street { get; set; } = string.Empty;

    [XmlAttribute("zip")]
    public string Zip { get; set; } = string.Empty;

    [XmlAttribute("telephone")]
    public string Telephone { get; set; } = string.Empty;
}
