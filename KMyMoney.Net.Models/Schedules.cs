namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Schedules : ArrayWithCount<ScheduledTransaction>
{
    [XmlElement("SCHEDULED_TX")]
    public override required ScheduledTransaction[] Values
    {
        get;
        set => field = value ?? [];
    } = [];
}
