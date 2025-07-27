namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class PayeeAddress
{
    [XmlAttribute("city")]
    public string City { get; init; } = string.Empty;

    [XmlAttribute("street")]
    public string Street { get; init; } = string.Empty;

    [XmlAttribute("postcode")]
    public string Postcode { get; init; } = string.Empty;

    [XmlAttribute("state")]
    public string State { get; init; } = string.Empty;

    [XmlAttribute("telephone")]
    public string Telephone { get; init; } = string.Empty;
}
