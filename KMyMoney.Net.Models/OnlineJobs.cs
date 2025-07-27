namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class OnlineJobs
{
    [XmlAttribute("count")]
    public int Count
    {
        get => 0;
        init { }
    }
}