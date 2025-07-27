namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("KMYMONEY-FILE")]
public class KmyMoneyFile
{
    [XmlElement("FILEINFO")]
    public required FileInfo FileInfo { get; init; }

    [XmlElement("USER")]
    public required User User { get; init; }

    [XmlElement("INSTITUTIONS")]
    public required Institutions Institutions { get; init; }

    [XmlElement("PAYEES")]
    public required Payees Payees { get; init; }

    [XmlElement("COSTCENTERS")]
    public required CostCenters CostCenters { get; init; }

    [XmlElement("TAGS")]
    public required Tags Tags { get; init; }

    [XmlElement("ACCOUNTS")]
    public required Accounts Accounts { get; init; }

    [XmlElement("TRANSACTIONS")]
    public required Transactions Transactions { get; init; }

    [XmlElement("KEYVALUEPAIRS")]
    public required KeyValuePairs KeyValuePairs { get; init; }

    [XmlElement("SCHEDULES")]
    public required Schedules Schedules { get; init; }

    [XmlElement("SECURITIES")]
    public required Securities Securities { get; init; }

    [XmlElement("CURRENCIES")]
    public required Currencies Currencies { get; init; }

    [XmlElement("PRICES")]
    public required Prices Prices { get; init; }

    [XmlElement("REPORTS")]
    public required Reports Reports { get; init; }

    [XmlElement("BUDGETS")]
    public required Budgets Budgets { get; init; }

    [XmlElement("ONLINEJOBS")]
    public required OnlineJobs OnlineJobs { get; init; }
}
