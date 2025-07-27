namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class InstitutionAddress
{
    [XmlAttribute("city")]
    public string City { get; init; } = string.Empty;

    [XmlAttribute("street")]
    public string Street { get; init; } = string.Empty;

    [XmlAttribute("zip")]
    public string Zip { get; init; } = string.Empty;

    [XmlAttribute("telephone")]
    public string Telephone { get; init; } = string.Empty;
}
