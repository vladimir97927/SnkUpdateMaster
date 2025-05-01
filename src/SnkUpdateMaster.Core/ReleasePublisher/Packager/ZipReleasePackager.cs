
using SnkUpdateMaster.Core.Integrity;
using System.IO.Compression;

namespace SnkUpdateMaster.Core.ReleasePublisher.Packager
{
    /// <summary>
    /// Реализация упаковщика релизов в ZIP-формат
    /// </summary>
    /// <param name="integrityProvider">Провайдер для вычисления контрольных сумм</param>
    public class ZipReleasePackager(IIntegrityProvider integrityProvider) : IReleasePackager
    {
        private readonly IIntegrityProvider _integrityProvider = integrityProvider;

        /// <summary>
        /// Создает ZIP-релиз из указанной директории
        /// </summary>
        /// <param name="sourceDir">Исходная директория с файлами для архивации</param>
        /// <param name="destDir">Целевая директория для сохранения ZIP-файла</param>
        /// <param name="version">Версия релиза</param>
        /// <param name="progress">Объект для отслеживания прогресса (0.0-1.0)</param>
        /// <returns>Объект <see cref="Release"/></returns>
        /// <exception cref="DirectoryNotFoundException">
        /// Исходная директория не существует
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Нет прав на запись в целевую директорию
        /// </exception>
        /// <exception cref="IOException">
        /// Ошибка при работе с файловой системой
        /// </exception>
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
