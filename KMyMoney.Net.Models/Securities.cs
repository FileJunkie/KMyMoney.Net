namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Securities
{
    [XmlAttribute("count")]
    public int Count
    {
        get => Security?.Length ?? 0;
        init { }
    }

    [XmlElement("SECURITY")]
    public Security[]? Security { get; set; }
}
