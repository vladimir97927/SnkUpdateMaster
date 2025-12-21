namespace SnkUpdateMaster.Core.Downloader
{
    /// <summary>
    /// Определяет интерфейс для загрузки файлов обновлений
    /// </summary>
    /// <remarks>
    /// Интерфейс предоставляет абстракцию для различных способов загрузки:
    /// <list type="bullet">
    /// <item><description>HTTP/HTTPS загрузка</description></item>
    /// <item><description>Загрузка из базы данных</description></item>
    /// <item><description>Локальное копирование</description></item>
    /// </list>
    /// </remarks>
    public interface IUpdateDownloader
    {
        /// <summary>
        /// Асинхронно загружает файл обновления
        /// </summary>
        /// <param name="updateInfo">Метаданные обновления</param>
        /// <param name="progress">Объект для отслеживания прогресса (0.0-1.0)</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>
        /// При успешном выполнении возвращает:
        /// <para>
        /// <see cref="string"/> - полный путь к загруженному файлу
        /// </para>
        /// </returns>
        Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, IProgress<double> progress, CancellationToken cancellationToken = default);
    }
}
