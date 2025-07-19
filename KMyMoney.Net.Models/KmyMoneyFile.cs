namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("KMYMONEY-FILE")]
public class KmyMoneyFile
{
    [XmlElement("FILEINFO")]
    public FileInfo? FileInfo { get; set; }

    [XmlElement("USER")]
    public User? User { get; set; }

    [XmlArray("INSTITUTIONS")]
    [XmlArrayItem("INSTITUTION")]
    public required Institution[] Institutions { get; set; } = [];

    [XmlArray("PAYEES")]
    [XmlArrayItem("PAYEE")]
    public required Payee[] Payees { get; set; } = [];

    [XmlArray("ACCOUNTS")]
    [XmlArrayItem("ACCOUNT")]
    public required Account[] Accounts { get; set; } = [];

    [XmlArray("TRANSACTIONS")]
    [XmlArrayItem("TRANSACTION")]
    public required Transaction[] Transactions { get; set; } = [];
}
