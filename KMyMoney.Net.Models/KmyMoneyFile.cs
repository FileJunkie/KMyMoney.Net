namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("KMYMONEY-FILE")]
public class KmyMoneyFile
{
    [XmlElement("FILEINFO")]
    public FileInfo? FileInfo { get; set; }

    [XmlElement("USER")]
    public User? User { get; set; }

    [XmlElement("INSTITUTIONS")]
    public required Institutions Institutions { get; set; }

    [XmlElement("PAYEES")]
    public required Payees Payees { get; set; }

    [XmlElement("COSTCENTERS")]
    public CostCenters? CostCenters { get; set; }

    [XmlElement("TAGS")]
    public Tags? Tags { get; set; }

    [XmlElement("ACCOUNTS")]
    public required Accounts Accounts { get; set; }

    [XmlElement("TRANSACTIONS")]
    public required Transactions Transactions { get; set; }

    [XmlElement("KEYVALUEPAIRS")]
    public KeyValuePairs? KeyValuePairs { get; set; }

    [XmlElement("SCHEDULES")]
    public Schedules? Schedules { get; set; }

    [XmlElement("SECURITIES")]
    public Securities? Securities { get; set; }

    [XmlElement("CURRENCIES")]
    public Currencies? Currencies { get; set; }

    [XmlElement("PRICES")]
    public Prices? Prices { get; set; }

    [XmlElement("REPORTS")]
    public Reports? Reports { get; set; }

    [XmlElement("BUDGETS")]
    public Budgets? Budgets { get; init; }

    [XmlElement("ONLINEJOBS")]
    public OnlineJobs? OnlineJobs { get; init; }
}
