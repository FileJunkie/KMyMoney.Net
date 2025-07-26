namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Tags
{
    [XmlAttribute("count")]
    public int Count
    {
        get => 0;
        init { }
    }
}
