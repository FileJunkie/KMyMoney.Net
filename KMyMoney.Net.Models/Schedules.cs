namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Schedules
{
    [XmlAttribute("count")]
    public int Count
    {
        get => ScheduledTransactions?.Length ?? 0;
        init { }
    }

    [XmlElement("SCHEDULED_TX")]
    public ScheduledTransaction[]? ScheduledTransactions { get; set; }
}
