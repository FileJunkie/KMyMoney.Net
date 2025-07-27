namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Payees : ArrayWithCount<Payee>
{
    [XmlElement("PAYEE")]
    public override required Payee[] Values { get; set; } = [];
}
