
using SnkUpdateMaster.Core.Integrity;
using System.IO.Compression;

namespace SnkUpdateMaster.Core.ReleasePublisher.Packager
{
    public class ZipReleasePackager(IIntegrityProvider integrityProvider) : IReleasePackager
    {
        private readonly IIntegrityProvider _integrityProvider = integrityProvider;

        public async Task<Release> PackAsync(string sourceDir, string destDir, Version version)
        {
            var destPath = Path.Combine(destDir, $"release-v{version}.zip");
            using (var archive = ZipFile.Open(destPath, ZipArchiveMode.Create))
                foreach (var file in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    var entryName = Path.GetRelativePath(sourceDir, file);
                    archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                }
            var checksum = _integrityProvider.ComputeFileChecksum(destPath);
            var fileName = Path.GetFileName(destPath);
            var fileData = await File.ReadAllBytesAsync(destPath);
            var release = Release.CreateNew(version, fileName, checksum, fileData);

            return release;
        }
    }
}
