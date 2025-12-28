
using SnkUpdateMaster.Core.Common;
using System.IO.Compression;

namespace SnkUpdateMaster.Core.Installer
{
    /// <summary>
    /// Реализация установщика обновлений из ZIP-архива
    /// </summary>
    /// <remarks>
    /// Обеспечивает установку через:
    /// <list type="number">
    /// <item><description>Создание полной резервной копии приложения</description></item>
    /// <item><description>Распаковку архива поверх текущей версии</description></item>
    /// <item><description>Автоматический откат при ошибках установки</description></item>
    /// </list>
    /// 
    /// Особенности реализации:
    /// <list type="bullet">
    /// <item><description>Резервная копия сохраняется в поддиректорию /backup</description></item>
    /// </list>
    /// </remarks>
    public class ZipInstaller : IInstaller
    {
        private readonly IDiskSpaceProvider _diskSpaceProvider;

        private readonly string _appDir;

        private readonly string _backupDir;

        /// <summary>
        /// Инициализирует новый экземпляр установщика
        /// </summary>
        /// <param name="appDir">Корневая директория приложения для обновления</param>
        /// <exception cref="ArgumentException">
        /// Возникает при недопустимом пути приложения
        /// </exception>
        public ZipInstaller(string appDir, IDiskSpaceProvider? diskSpaceProvider = null)
        {
            _diskSpaceProvider = diskSpaceProvider ?? new DiskSpaceProvider();
            _appDir = Path.GetFullPath(appDir);
            var appFolderName = Path.GetFileName(Path.TrimEndingDirectorySeparator(_appDir));
            var backupBaseDir = Path.GetDirectoryName(appDir) ?? Path.GetTempPath();
            _backupDir = Path.Combine(backupBaseDir, $"{appFolderName}_backup");
        }

        /// <summary>
        /// Выполняет установку обновления из ZIP-файла
        /// </summary>
        /// <param name="updateFilePath">Путь к архиву обновления</param>
        /// <param name="progress">Объект для отслеживания прогресса (0.0-1.0)</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="InvalidDataException">
        /// Некорректный формат ZIP-архива
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Отсутствуют права на запись в целевую директорию
        /// </exception>
        public async Task InstallAsync(string updateFilePath, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            using var archive = ZipFile.OpenRead(updateFilePath);
            EnsureSufficientDiskSpace(archive);

            try
            {
                await CreateBackupAsync(progress, cancellationToken);
                await ExtractUpdateAsync(archive, progress, cancellationToken);
                CleanupBackup();
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        private void EnsureSufficientDiskSpace(ZipArchive archive)
        {
            var requiredSpace = archive.Entries.Sum(e => e.Length);
            var availableSpace = _diskSpaceProvider.GetAvailableFreeSpace(_appDir);

            if (availableSpace < requiredSpace)
            {
                throw new IOException("There is not enough free disk space to install updates." +
                    $"{requiredSpace} bytes required, {availableSpace} bytes available.");
            }
        }

        private Task CreateBackupAsync(IProgress<double>? progress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                CleanupBackup();
                Directory.CreateDirectory(_backupDir);
                var files = Directory.GetFiles(_appDir, "*", SearchOption.AllDirectories);
                var filesCount = files.Length;
                for (var i = 0; i < filesCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var file = files[i];
                    var relativePath = Path.GetRelativePath(_appDir, file);
                    var backupPath = Path.Combine(_backupDir, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath)!);
                    File.Copy(file, backupPath, true);

                    progress?.Report((double)(i + 1) / filesCount * 0.5);
                }

                if (filesCount == 0)
                {
                    progress?.Report(0.5);
                }
            }, cancellationToken);
        }

        private Task ExtractUpdateAsync(ZipArchive archive, IProgress<double>? progress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var totalEntries = archive.Entries.Count;
                if (totalEntries == 0)
                {
                    progress?.Report(1.0);
                    return;
                }
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

                    progress?.Report(0.5 + (double)(i + 1) / totalEntries * 0.5);
                }
            }, cancellationToken);
        }

        private void Rollback()
        {
            if (!Directory.Exists(_backupDir))
                return;
            try
            {
                if (Directory.Exists(_appDir))
                {
                    Directory.Delete(_appDir, true);
                }

                Directory.Move(_backupDir, _appDir);
            }
            catch
            {
                throw;
            }
        }

        private void CleanupBackup()
        {
            try
            {
                if (Directory.Exists(_backupDir))
                {
                    Directory.Delete(_backupDir, true);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
