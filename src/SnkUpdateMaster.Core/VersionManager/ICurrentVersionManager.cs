namespace SnkUpdateMaster.Core.VersionManager
{
    /// <summary>
    /// Определяет интерфейс для управления текущей версией приложения
    /// </summary>
    /// <remarks>
    /// Интерфейс предоставляет абстракцию для работы с версией приложения:
    /// <list type="bullet">
    /// <item><description>Чтение текущей установленной версии</description></item>
    /// <item><description>Обновление информации о версии после успешной установки</description></item>
    /// </list>
    /// Реализации могут использовать различные хранилища:
    /// <list type="bullet">
    /// <item><description>Файл конфигурации</description></item>
    /// <item><description>Метаданные сборки</description></item>
    /// </list>
    /// </remarks>
    public interface ICurrentVersionManager
    {
        /// <summary>
        /// Асинхронно получает текущую версию приложения
        /// </summary>
        /// <returns>
        /// При успешном выполнении содержит:
        /// <para>
        /// <see cref="Version"/> - текущая версия приложения<br/>
        /// <see langword="null"/> - если версия не определена
        /// </para>
        /// </returns>
        Task<Version?> GetCurrentVersionAsync();

        /// <summary>
        /// Асинхронно обновляет информацию о текущей версии приложения
        /// </summary>
        /// <param name="version">Новая версия приложения</param>
        Task UpdateCurrentVersionAsync(Version version);
    }
}
