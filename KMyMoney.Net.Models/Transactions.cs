namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Transactions
{
    [XmlAttribute("count")]
    public int Count { get; set; }

    [XmlElement("TRANSACTION")]
    public required Transaction[] Transaction { get; set; } = [];
}
