namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("FILEINFO")]
public class FileInfo
{
    [XmlElement("CREATION_DATE")]
    public CreationDate? CreationDate { get; init; }

    [XmlElement("LAST_MODIFIED_DATE")]
    public LastModifiedDate? LastModifiedDate { get; init; }

    [XmlElement("VERSION")]
    public Version? Version { get; init; }

    [XmlElement("FIXVERSION")]
    public FixVersion? FixVersion { get; init; }
}
