namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Transactions : ArrayWithCount<Transaction>
{
    [XmlElement("TRANSACTION")]
    public override required Transaction[] Values { get; set; } = [];
}
