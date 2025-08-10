namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class KeyValuePairs
{
    [XmlElement("PAIR")]
    public required KeyValuePair[] Pair
    {
        get;
        set => field = value ?? [];
    } = [];
}
