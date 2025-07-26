namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Accounts
{
    [XmlAttribute("count")]
    public int Count
    {
        get => Account.Length;
        init { }
    }

    [XmlElement("ACCOUNT")]
    public required Account[] Account { get; set; } = [];
}
