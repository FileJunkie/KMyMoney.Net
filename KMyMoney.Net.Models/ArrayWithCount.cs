using System.Xml.Serialization;

namespace KMyMoney.Net.Models;

public abstract class ArrayWithCount<T>
{
    [XmlAttribute("count")]
    public int Count
    {
        get => Values.Length;
        init { }
    }

    [XmlIgnore]
    public abstract T[] Values { get; set; }
}