namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Accounts : ArrayWithCount<Account>
{
    [XmlElement("ACCOUNT")]
    public override required Account[] Values { get; set; } = [];
}
