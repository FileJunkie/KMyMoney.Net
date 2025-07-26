namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("SCHEDULED_TX")]
public class ScheduledTransaction
{
    [XmlAttribute("paymentType")]
    public required string PaymentType { get; set; }

    [XmlAttribute("name")]
    public required string Name { get; set; }

    [XmlAttribute("endDate")]
    public string? EndDate { get; set; }

    [XmlAttribute("type")]
    public required string Type { get; set; }

    [XmlAttribute("occurence")]
    public required string Occurence { get; set; }

    [XmlAttribute("weekendOption")]
    public required string WeekendOption { get; set; }

    [XmlAttribute("autoEnter")]
    public required string AutoEnter { get; set; }

    [XmlAttribute("fixed")]
    public required string Fixed { get; set; }

    [XmlAttribute("startDate")]
    public required string StartDate { get; set; }

    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("lastDayInMonth")]
    public required string LastDayInMonth { get; set; }

    [XmlAttribute("lastPayment")]
    public string? LastPayment { get; set; }

    [XmlAttribute("occurenceMultiplier")]
    public required string OccurenceMultiplier { get; set; }

    [XmlElement("PAYMENTS")]
    public Payments? Payments { get; set; }

    [XmlElement("TRANSACTION")]
    public required Transaction Transaction { get; set; }
}
