
using System.IO.Compression;

namespace SnkUpdateMaster.Core.Installer
{
    public class ZipInstaller : IInstaller
    {
        private readonly string _appDir;

        private readonly string _backupDir;

        public ZipInstaller(string appDir)
        {
            _appDir = appDir;
            _backupDir = Path.Combine(_appDir, "backup");
        }

        public async Task InstallAsync(string updateFilePath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            try
            {
                await CreateBackupAsync(progress, cancellationToken);
                await ExtractUpdateAsync(updateFilePath, progress, cancellationToken);
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        private Task CreateBackupAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Directory.CreateDirectory(_backupDir);
                var files = Directory.GetFiles(_appDir, "*", SearchOption.AllDirectories);
                for (var i = 0; i < files.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var file = files[i];
                    var relativePath = Path.GetRelativePath(_appDir, file);
                    var backupPath = Path.Combine(_backupDir, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath)!);
                    File.Copy(file, backupPath, true);

                    progress.Report((double)i / files.Length * 0.5);
                }
            }, cancellationToken);
        }

        private Task ExtractUpdateAsync(string updateFilePath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                using var archive = ZipFile.OpenRead(updateFilePath);
                var totalEntries = archive.Entries.Count;
                for (int i = 0; i < totalEntries; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var entry = archive.Entries[i];
                    var destPath = Path.Combine(_appDir, entry.FullName);
                    if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                        entry.ExtractToFile(destPath, true);
                    }

                    progress.Report(0.5 + (double)i / totalEntries * 0.5);
                }
            }, cancellationToken);
        }

        private void Rollback()
        {
            if (!Directory.Exists(_backupDir))
                return;
            try
            {
                Directory.Delete(_appDir, true);
                Directory.Move(_backupDir, _appDir);
            }
            catch
            {
                throw;
            }
        }
    }
}
