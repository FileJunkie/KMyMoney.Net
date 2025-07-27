namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Securities : ArrayWithCount<Security>
{
    [XmlElement("SECURITY")]
    public override required Security[] Values { get; set; } = [];
}
