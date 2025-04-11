
using SnkUpdateMaster.Core.Integrity;
using System.IO.Compression;

namespace SnkUpdateMaster.Core.ReleasePublisher.Packager
{
    public class ZipReleasePackager(IIntegrityProvider integrityProvider) : IReleasePackager
    {
        private readonly IIntegrityProvider _integrityProvider = integrityProvider;

        public async Task<Release> PackAsync(string sourceDir, string destDir, Version version, IProgress<double> progress)
        {
            var destPath = Path.Combine(destDir, $"release-v{version}.zip");
            var files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
            using (var archive = ZipFile.Open(destPath, ZipArchiveMode.Create))
                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    var entryName = Path.GetRelativePath(sourceDir, file);
                    archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                    progress.Report((double)i / files.Length * 0.5);
                }
            var checksum = _integrityProvider.ComputeFileChecksum(destPath);
            var fileName = Path.GetFileName(destPath);
            var fileData = await File.ReadAllBytesAsync(destPath);
            var release = Release.CreateNew(version, fileName, checksum, fileData);

            return release;
        }
    }
}
