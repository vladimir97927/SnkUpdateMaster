
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
        private readonly string _appDir;

        private readonly string _backupDir;

        /// <summary>
        /// Инициализирует новый экземпляр установщика
        /// </summary>
        /// <param name="appDir">Корневая директория приложения для обновления</param>
        /// <exception cref="ArgumentException">
        /// Возникает при недопустимом пути приложения
        /// </exception>
        public ZipInstaller(string appDir)
        {
            _appDir = appDir;
            _backupDir = Path.Combine(_appDir, "backup");
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
        public async Task InstallAsync(string updateFilePath, IProgress<double> progress, CancellationToken cancellationToken = default)
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
