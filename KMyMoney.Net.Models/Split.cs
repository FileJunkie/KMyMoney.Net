namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Split
{
    [XmlAttribute("reconciledate")]
    public string ReconcileDate { get; set; } = string.Empty;

    [XmlAttribute("bankid")]
    public string BankId { get; set; } = string.Empty;

    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("payee")]
    public string Payee { get; set; } = string.Empty;

    [XmlAttribute("action")]
    public string Action { get; set; } = string.Empty;

    [XmlAttribute("price")]
    public string Price { get; set; } = string.Empty;

    [XmlAttribute("memo")]
    public string Memo { get; set; } = string.Empty;

    [XmlAttribute("number")]
    public string Number { get; set; } = string.Empty;

    [XmlAttribute("value")]
    public required string Value { get; set; }

    [XmlAttribute("account")]
    public required string Account { get; set; }

    [XmlAttribute("reconcileflag")]
    public string ReconcileFlag { get; set; } = string.Empty;

    [XmlAttribute("shares")]
    public required string Shares { get; set; }
}
