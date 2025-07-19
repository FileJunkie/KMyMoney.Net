namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Split
{
    [XmlAttribute("payee")]
    public string? Payee { get; set; }

    [XmlAttribute("reconcileflag")]
    public string? ReconcileFlag { get; set; }

    [XmlAttribute("account")]
    public required string Account { get; set; }

    [XmlAttribute("reconciledate")]
    public string? ReconcileDate { get; set; }

    [XmlAttribute("shares")]
    public required string Shares { get; set; }

    [XmlAttribute("value")]
    public required string Value { get; set; }

    [XmlAttribute("memo")]
    public string? Memo { get; set; }

    [XmlAttribute("number")]
    public string? Number { get; set; }

    [XmlAttribute("bankid")]
    public string? BankId { get; set; }
}
