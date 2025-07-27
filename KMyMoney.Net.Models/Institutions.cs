namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Institutions : ArrayWithCount<Institution>
{
    [XmlElement("INSTITUTION")]
    public override required Institution[] Values { get; set; } = [];
}
