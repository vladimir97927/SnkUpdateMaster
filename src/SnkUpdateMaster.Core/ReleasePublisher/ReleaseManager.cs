using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.ReleasePublisher.Packager;

namespace SnkUpdateMaster.Core.ReleasePublisher
{
    /// <summary>
    /// Класс управления релизами приложенияя
    /// </summary>
    /// <remarks>
    /// <para><strong>Зависимости:</strong></para>
    /// <param name="releasePackager">Упаковщик релизов</param>
    /// <param name="releaseSource">Хранилище релизов</param>
    /// <param name="releaseInfoSource">Источник информации о релизах</param>
    /// </remarks>
    public class ReleaseManager(
        IReleasePackager releasePackager,
        IReleaseSource releaseSource,
        IReleaseInfoSource releaseInfoSource)
    {
        private readonly IReleasePackager _releasePackager = releasePackager;

        private readonly IReleaseSource _releaseSource = releaseSource;

        private readonly IReleaseInfoSource _releaseInfoSource = releaseInfoSource;

        /// <summary>
        /// Публикует новый релиз приложения
        /// </summary>
        /// <param name="appDir">Директория с файлами приложения</param>
        /// <param name="version">Версия релиза</param>
        /// <param name="progress">Объект для отслеживания прогресса (0.0-1.0)</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>ID созданного релиза</returns>
        public async Task<int> PulishReleaseAsync(string appDir, Version version, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            var destPath = Path.Combine(Environment.CurrentDirectory, "Releases");
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            progress.Report(0.2);
            var packProgress = new Progress<double>(p => progress.Report(p * 0.5));
            var release = await _releasePackager.PackAsync(appDir, destPath, version, packProgress);
            await _releaseSource.UploadReleaseAsync(release);
            progress.Report(1);

            return release.Id;
        }

        /// <summary>
        /// Получает страничный список информации о релизах
        /// </summary>
        /// <param name="page">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns>Пагинированные данные с информацией о релизах</returns>
        public Task<PagedData<IEnumerable<ReleaseInfo>>> GetReleaseInfosPagedAsync(int? page = null, int? pageSize = null)
        {
            return _releaseInfoSource.GetReleaseInfosPagedAsync(page, pageSize);
        }
    }
}
