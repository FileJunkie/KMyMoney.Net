namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

public class Report
{
    [XmlAttribute("chartbydefault")]
    public string? ChartByDefault { get; set; }

    [XmlAttribute("chartchgridlines")]
    public string? ChartChGridLines { get; set; }

    [XmlAttribute("chartdatalabels")]
    public string? ChartDataLabels { get; set; }

    [XmlAttribute("chartlinewidth")]
    public string? ChartLineWidth { get; set; }

    [XmlAttribute("chartpalette")]
    public string? ChartPalette { get; set; }

    [XmlAttribute("chartsvgridlines")]
    public string? ChartSvGridLines { get; set; }

    [XmlAttribute("charttype")]
    public string? ChartType { get; set; }

    [XmlAttribute("columnsaredays")]
    public string? ColumnsAreDays { get; set; }

    [XmlAttribute("columntype")]
    public string? ColumnType { get; set; }

    [XmlAttribute("comment")]
    public string? Comment { get; set; }

    [XmlAttribute("convertcurrency")]
    public string? ConvertCurrency { get; set; }

    [XmlAttribute("datalock")]
    public string? DataLock { get; set; }

    [XmlAttribute("dataMajorTick")]
    public string? DataMajorTick { get; set; }

    [XmlAttribute("dataMinorTick")]
    public string? DataMinorTick { get; set; }

    [XmlAttribute("dataRangeEnd")]
    public string? DataRangeEnd { get; set; }

    [XmlAttribute("dataRangeStart")]
    public string? DataRangeStart { get; set; }

    [XmlAttribute("datelock")]
    public string? DateLock { get; set; }

    [XmlAttribute("detail")]
    public string? Detail { get; set; }

    [XmlAttribute("favorite")]
    public string? Favorite { get; set; }

    [XmlAttribute("group")]
    public string? Group { get; set; }

    [XmlAttribute("hidetransactions")]
    public string? HideTransactions { get; set; }

    [XmlAttribute("id")]
    public string? Id { get; set; }

    [XmlAttribute("includesactuals")]
    public string? IncludesActuals { get; set; }

    [XmlAttribute("includesaverageprice")]
    public string? IncludesAveragePrice { get; set; }

    [XmlAttribute("includeschedules")]
    public string? IncludeSchedules { get; set; }

    [XmlAttribute("includesforecast")]
    public string? IncludesForecast { get; set; }

    [XmlAttribute("includesmovingaverage")]
    public string? IncludesMovingAverage { get; set; }

    [XmlAttribute("includesprice")]
    public string? IncludesPrice { get; set; }

    [XmlAttribute("includestransfers")]
    public string? IncludeTransfers { get; set; }

    [XmlAttribute("includeunused")]
    public string? IncludeUnused { get; set; }

    [XmlAttribute("investments")]
    public string? Investments { get; set; }

    [XmlAttribute("logYaxis")]
    public string? LogYaxis { get; set; }

    [XmlAttribute("mixedtime")]
    public string? MixedTime { get; set; }

    [XmlAttribute("name")]
    public string? Name { get; set; }

    [XmlAttribute("negexpenses")]
    public string? NegExpenses { get; set; }

    [XmlAttribute("rowtype")]
    public string? RowType { get; set; }

    [XmlAttribute("showcolumntotals")]
    public string? ShowColumnTotals { get; set; }

    [XmlAttribute("showrowtotals")]
    public string? ShowRowTotals { get; set; }

    [XmlAttribute("skipZero")]
    public string? SkipZero { get; set; }

    [XmlAttribute("type")]
    public string? Type { get; set; }

    [XmlAttribute("yLabelsPrecision")]
    public string? YLabelsPrecision { get; set; }

    [XmlElement("ACCOUNTGROUP")]
    public AccountGroup[]? AccountGroups { get; set; }
}
