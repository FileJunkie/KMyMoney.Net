namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Report
{
    [XmlAttribute("count")]
    public required string Type { get; init; }

    [XmlAttribute("detail")]
    public required string Detail { get; init; }

    [XmlAttribute("investments")]
    public required string Investments { get; init; }

    [XmlAttribute("hidetransactions")]
    public required string HideTransactions { get; init; }

    [XmlAttribute("rowtype")]
    public required string RowType { get; init; }

    [XmlAttribute("loans")]
    public required string Loans { get; init; }

    [XmlAttribute("group")]
    public required string Group { get; init; }

    [XmlAttribute("datelock")]
    public required string DateLock { get; init; }

    [XmlAttribute("investmentsum")]
    public required string InvestmentSum { get; init; }

    [XmlAttribute("tax")]
    public required string Tax { get; init; }

    [XmlAttribute("includestransfers")]
    public required string IncludesTransfers { get; init; }

    [XmlAttribute("name")]
    public required string Name { get; init; }

    [XmlAttribute("skipZero")]
    public required string SkipZero { get; init; }

    [XmlAttribute("convertcurrency")]
    public required string ConvertCurrency { get; init; }

    [XmlAttribute("id")]
    public required string Id { get; init; }

    [XmlAttribute("comment")]
    public required string Comment { get; init; }

    [XmlAttribute("favorite")]
    public required string Favorite { get; init; }

    [XmlAttribute("querycolumns")]
    public required string QueryColumns { get; init; }

    [XmlAttribute("showcolumntotal")]
    public required string ShowColumnTotal { get; init; }

    [XmlElement("ACCOUNTGROUP")]
    public required AccountGroup[] AccountGroups { get; set; } = [];
}
