namespace PCMS.UCEDockets.Common;

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;
using Xml.Ucms.Criminaldockets;

public static class UCEDocketsSerializer
{

    public static CriminalDockets Parse(string archiveLocalFileName)
    {
        using var fs = File.Open(archiveLocalFileName, FileMode.Open);
        return Parse(fs);
    }

    public static CriminalDockets Parse(Stream archiveStream)
    {
        var archive = new ZipArchive(archiveStream);

        if (archive.Entries.Count != 1)
            throw new Exception("number of files in archive is not == 1");

        var entry = archive.Entries.First();
        using var xmlStream = entry.Open();
        var serializer = new XmlSerializer(typeof(CriminalDockets));
        return serializer.Deserialize(xmlStream) as CriminalDockets;
    }

    public static string SerializeDocket(CriminalDocketsDistrictCountyCourtDocket docket)
    {
        var serializer = new XmlSerializer(typeof(CriminalDocketsDistrictCountyCourtDocket));
        var sw = new StringWriter();
        serializer.Serialize(sw, docket);

        return sw.ToString();
    }

    public static CriminalDocketsDistrictCountyCourtDocket DeserializeDocket(string xml)
    {
        var serializer = new XmlSerializer(typeof(CriminalDocketsDistrictCountyCourtDocket));
        using var sr = new StringReader(xml);
        return serializer.Deserialize(sr) as CriminalDocketsDistrictCountyCourtDocket;
    }
}