namespace KMyMoney.Net.Models;

using System.Xml.Serialization;

[XmlRoot("FILEINFO")]
public class FileInfo
{
    [XmlElement("CREATION_DATE")]
    public required CreationDate CreationDate { get; set; }

    [XmlElement("LAST_MODIFIED_DATE")]
    public required LastModifiedDate LastModifiedDate { get; set; }

    [XmlElement("VERSION")]
    public required Version Version { get; set; }

    [XmlElement("FIXVERSION")]
    public required FixVersion FixVersion { get; set; }
}
