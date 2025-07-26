namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("FILEINFO")]
public class FileInfo
{
    [XmlElement("CREATION_DATE")]
    public CreationDate? CreationDate { get; set; }

    [XmlElement("LAST_MODIFIED_DATE")]
    public LastModifiedDate? LastModifiedDate { get; set; }

    [XmlElement("VERSION")]
    public Version? Version { get; set; }

    [XmlElement("FIXVERSION")]
    public FixVersion? FixVersion { get; set; }
}
