namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Reports : ArrayWithCount<Report>
{
    [XmlElement("REPORT")]
    public override required Report[] Values { get; set; } = [];
}
