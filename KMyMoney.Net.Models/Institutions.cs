namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Institutions
{
    [XmlAttribute("count")]
    public int Count
    {
        get => Institution.Length;
        init { }
    }

    [XmlElement("INSTITUTION")]
    public required Institution[] Institution { get; set; } = [];
}
