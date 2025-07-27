namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Report
{
    [XmlAttribute("chartbydefault")]
    public string? ChartByDefault { get; init; }

    [XmlAttribute("chartchgridlines")]
    public string? ChartChGridLines { get; init; }

    [XmlAttribute("chartdatalabels")]
    public string? ChartDataLabels { get; init; }

    [XmlAttribute("chartlinewidth")]
    public string? ChartLineWidth { get; init; }

    [XmlAttribute("chartpalette")]
    public string? ChartPalette { get; init; }

    [XmlAttribute("chartsvgridlines")]
    public string? ChartSvGridLines { get; init; }

    [XmlAttribute("charttype")]
    public string? ChartType { get; init; }

    [XmlAttribute("columnsaredays")]
    public string? ColumnsAreDays { get; init; }

    [XmlAttribute("columntype")]
    public string? ColumnType { get; init; }

    [XmlAttribute("comment")]
    public string? Comment { get; init; }

    [XmlAttribute("convertcurrency")]
    public string? ConvertCurrency { get; init; }

    [XmlAttribute("datalock")]
    public string? DataLock { get; init; }

    [XmlAttribute("dataMajorTick")]
    public string? DataMajorTick { get; init; }

    [XmlAttribute("dataMinorTick")]
    public string? DataMinorTick { get; init; }

    [XmlAttribute("dataRangeEnd")]
    public string? DataRangeEnd { get; init; }

    [XmlAttribute("dataRangeStart")]
    public string? DataRangeStart { get; init; }

    [XmlAttribute("datelock")]
    public string? DateLock { get; init; }

    [XmlAttribute("detail")]
    public string? Detail { get; init; }

    [XmlAttribute("favorite")]
    public string? Favorite { get; init; }

    [XmlAttribute("group")]
    public string? Group { get; init; }

    [XmlAttribute("hidetransactions")]
    public string? HideTransactions { get; init; }

    [XmlAttribute("id")]
    public string? Id { get; init; }

    [XmlAttribute("includesactuals")]
    public string? IncludesActuals { get; init; }

    [XmlAttribute("includesaverageprice")]
    public string? IncludesAveragePrice { get; init; }

    [XmlAttribute("includeschedules")]
    public string? IncludeSchedules { get; init; }

    [XmlAttribute("includesforecast")]
    public string? IncludesForecast { get; init; }

    [XmlAttribute("includesmovingaverage")]
    public string? IncludesMovingAverage { get; init; }

    [XmlAttribute("includesprice")]
    public string? IncludesPrice { get; init; }

    [XmlAttribute("includestransfers")]
    public string? IncludeTransfers { get; init; }

    [XmlAttribute("includeunused")]
    public string? IncludeUnused { get; init; }

    [XmlAttribute("investments")]
    public string? Investments { get; init; }

    [XmlAttribute("logYaxis")]
    public string? LogYaxis { get; init; }

    [XmlAttribute("mixedtime")]
    public string? MixedTime { get; init; }

    [XmlAttribute("name")]
    public string? Name { get; init; }

    [XmlAttribute("negexpenses")]
    public string? NegExpenses { get; init; }

    [XmlAttribute("rowtype")]
    public string? RowType { get; init; }

    [XmlAttribute("showcolumntotals")]
    public string? ShowColumnTotals { get; init; }

    [XmlAttribute("showrowtotals")]
    public string? ShowRowTotals { get; init; }

    [XmlAttribute("skipZero")]
    public string? SkipZero { get; init; }

    [XmlAttribute("type")]
    public string? Type { get; init; }

    [XmlAttribute("yLabelsPrecision")]
    public string? YLabelsPrecision { get; init; }

    [XmlElement("ACCOUNTGROUP")]
    public AccountGroup[]? AccountGroups { get; init; }
}
