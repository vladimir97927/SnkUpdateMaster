using System.IO.Compression;

namespace SnkUpdateMaster.Core
{
    public class ZipPackage
    {
        public ZipPackage(string filePath)
        {
            using var zipStream = File.OpenRead(filePath);
            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read, false);
        }
    }
}
