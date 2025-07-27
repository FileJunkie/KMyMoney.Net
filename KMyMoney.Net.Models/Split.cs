namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Split
{
    [XmlAttribute("reconciledate")]
    public string ReconcileDate { get; init; } = string.Empty;

    [XmlAttribute("bankid")]
    public string BankId { get; init; } = string.Empty;

    [XmlAttribute("id")]
    public required string Id { get; init; }

    [XmlAttribute("payee")]
    public string Payee { get; init; } = string.Empty;

    [XmlAttribute("action")]
    public string Action { get; init; } = string.Empty;

    [XmlAttribute("price")]
    public string Price { get; init; } = string.Empty;

    [XmlAttribute("memo")]
    public string Memo { get; init; } = string.Empty;

    [XmlAttribute("number")]
    public string Number { get; init; } = string.Empty;

    [XmlAttribute("value")]
    public required string Value { get; init; }

    [XmlAttribute("account")]
    public required string Account { get; init; }

    [XmlAttribute("reconcileflag")]
    public string ReconcileFlag { get; init; } = string.Empty;

    [XmlAttribute("shares")]
    public required string Shares { get; init; }
}
