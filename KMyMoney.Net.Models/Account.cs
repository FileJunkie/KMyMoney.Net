namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("ACCOUNT")]
public class Account : IHasId, IHasName
{
    [XmlAttribute("opened")]
    public string? Opened { get; init; }

    [XmlAttribute("name")]
    public required string Name { get; init; }

    [XmlAttribute("lastreconciled")]
    public string? LastReconciled { get; init; }

    [XmlAttribute("lastmodified")]
    public string? LastModified { get; init; }

    [XmlAttribute("type")]
    public required string Type { get; init; }

    [XmlAttribute("parentaccount")]
    public string? ParentAccount { get; init; }

    [XmlAttribute("id")]
    public required string Id { get; init; }

    [XmlAttribute("currency")]
    public required string Currency { get; init; }

    [XmlAttribute("institution")]
    public string? Institution { get; init; }

    [XmlAttribute("number")]
    public string? Number { get; init; }

    [XmlAttribute("description")]
    public string? Description { get; init; }

    [XmlArray("SUBACCOUNTS")]
    [XmlArrayItem("SUBACCOUNT")]
    public SubAccount[]? SubAccounts { get; init; }

    [XmlElement("KEYVALUEPAIRS")]
    public KeyValuePairs? KeyValuePairs { get; init; }
}
