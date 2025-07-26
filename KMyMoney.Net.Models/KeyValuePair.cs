namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("PAIR")]
public class KeyValuePair
{
    [XmlAttribute("key")]
    public required string Key { get; set; }

    [XmlAttribute("value")]
    public required string Value { get; set; }
}
