namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class CreationDate
{
    [XmlAttribute("date")]
    public required string Date { get; init; }
}
