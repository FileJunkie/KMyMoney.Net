namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Currencies
{
    [XmlAttribute("count")]
    public int Count
    {
        get => Currency.Length;
        init { }
    }

    [XmlElement("CURRENCY")]
    public required Currency[] Currency { get; set; } = [];
}
