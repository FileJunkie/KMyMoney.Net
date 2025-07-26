namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Payees
{
    [XmlAttribute("count")]
    public int Count
    {
        get => Payee.Length;
        init { }
    }

    [XmlElement("PAYEE")]
    public required Payee[] Payee { get; set; } = [];
}
