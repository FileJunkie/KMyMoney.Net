namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class LastModifiedDate
{
    [XmlAttribute("date")]
    public required string Date { get; set; }
}
