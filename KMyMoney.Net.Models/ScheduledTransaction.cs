namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("SCHEDULED_TX")]
public class ScheduledTransaction
{
    [XmlAttribute("paymentType")]
    public required string PaymentType { get; init; }

    [XmlAttribute("name")]
    public required string Name { get; init; }

    [XmlAttribute("endDate")]
    public string? EndDate { get; init; }

    [XmlAttribute("type")]
    public required string Type { get; init; }

    [XmlAttribute("occurence")]
    public required string Occurence { get; init; }

    [XmlAttribute("weekendOption")]
    public required string WeekendOption { get; init; }

    [XmlAttribute("autoEnter")]
    public required string AutoEnter { get; init; }

    [XmlAttribute("fixed")]
    public required string Fixed { get; init; }

    [XmlAttribute("startDate")]
    public required string StartDate { get; init; }

    [XmlAttribute("id")]
    public required string Id { get; init; }

    [XmlAttribute("lastDayInMonth")]
    public required string LastDayInMonth { get; init; }

    [XmlAttribute("lastPayment")]
    public string? LastPayment { get; init; }

    [XmlAttribute("occurenceMultiplier")]
    public required string OccurenceMultiplier { get; init; }

    [XmlElement("PAYMENTS")]
    public Payments? Payments { get; init; }

    [XmlElement("TRANSACTION")]
    public required Transaction Transaction { get; init; }
}
