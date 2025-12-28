namespace SnkUpdateMaster.Core.Installer
{
    /// <summary>
    /// Определяет интерфейс для установки обновлений приложения
    /// </summary>
    /// <remarks>
    /// Интерфейс предоставляет абстракцию для различных сценариев установки:
    /// <list type="bullet">
    /// <item><description>Распаковка и установка из ZIP-архива</description></item>
    /// <item><description>Запуск исполняемого инсталлятора</description></item>
    /// </list>
    /// </remarks>
    public interface IInstaller
    {
        /// <summary>
        /// Асинхронно выполняет установку обновления
        /// </summary>
        /// <param name="updateFilePath">
        /// Полный путь к файлу обновления. Должен существовать и быть доступным для чтения
        /// </param>
        /// <param name="progress">
        /// Объект для отслеживания прогресса установки (0.0-1.0)
        /// </param>
        /// <param name="cancellationToken">
        /// Токен для отмены операции
        /// </param>
        Task InstallAsync(string updateFilePath, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
    }
}
