namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("ACCOUNT")]
public class Account
{
    [XmlAttribute("institution")]
    public string? Institution { get; set; }

    [XmlAttribute("number")]
    public string? Number { get; set; }

    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("parentaccount")]
    public string? ParentAccount { get; set; }

    [XmlAttribute("opened")]
    public string? Opened { get; set; }

    [XmlAttribute("name")]
    public required string Name { get; set; }

    [XmlAttribute("lastreconciled")]
    public string? LastReconciled { get; set; }

    [XmlAttribute("currency")]
    public required string Currency { get; set; }

    [XmlAttribute("lastmodified")]
    public string? LastModified { get; set; }

    [XmlAttribute("type")]
    public required string Type { get; set; }

    [XmlAttribute("description")]
    public string? Description { get; set; }

    [XmlArray("SUBACCOUNTS")]
    [XmlArrayItem("SUBACCOUNT")]
    public required SubAccount[] SubAccounts { get; set; } = [];
}
