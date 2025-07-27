namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Splits
{
    [XmlElement("SPLIT")]
    public required Split[] Split { get; init; }
}
