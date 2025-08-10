namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Prices : ArrayWithCount<PricePair>
{
    [XmlElement("PRICEPAIR")]
    public override required PricePair[] Values
    {
        get;
        set => field = value ?? [];
    } = [];
}
