namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Reports
{
    [XmlAttribute("count")]
    public int Count
    {
        get => Report.Length;
        init { }
    }

    [XmlElement("REPORT")]
    public required Report[] Report { get; set; } = [];
}
