namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Currencies : ArrayWithCount<Currency>
{
    [XmlElement("CURRENCY")]
    public override required Currency[] Values
    {
        get;
        set => field = value ?? [];
    } = [];
}
