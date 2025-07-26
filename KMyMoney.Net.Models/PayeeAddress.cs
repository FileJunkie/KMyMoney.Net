namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class PayeeAddress
{
    [XmlAttribute("city")]
    public string City { get; set; } = string.Empty;

    [XmlAttribute("street")]
    public string Street { get; set; } = string.Empty;

    [XmlAttribute("postcode")]
    public string Postcode { get; set; } = string.Empty;

    [XmlAttribute("state")]
    public string State { get; set; } = string.Empty;

    [XmlAttribute("telephone")]
    public string Telephone { get; set; } = string.Empty;
}
