using System.Formats.Tar;
using System.IO.Compression;

namespace NanosSharp.Updater;

public static class ArchiveExtractor
{
    public static void ExtractTarGz(string archive, string directory)
    {
        using var file = File.OpenRead(archive);
        using var gzip = new GZipStream(file, CompressionMode.Decompress);
        TarFile.ExtractToDirectory(gzip, directory, overwriteFiles: true);
    }

    public static void ExtractZip(string archive, string directory) =>
        ZipFile.ExtractToDirectory(archive, directory, overwriteFiles: true);
}
