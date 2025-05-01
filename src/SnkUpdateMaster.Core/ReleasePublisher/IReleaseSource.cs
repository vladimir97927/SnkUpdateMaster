namespace SnkUpdateMaster.Core.ReleasePublisher
{
    /// <summary>
    /// Интерфейс для работы с хранилищем релизов
    /// </summary>
    /// <remarks>
    /// Интерфейс предоставляет абстракцию для:
    /// <list type="bullet">
    /// <item><description>Загрузки новых версий в хранилище</description></item>
    /// <item><description>Получения информации о существующих релизах</description></item>
    /// <item><description>Управления жизненным циклом релизов</description></item>
    /// </list>
    /// </remarks>
    public interface IReleaseSource
    {
        /// <summary>
        /// Асинхронно загружает новый релиз в хранилище
        /// </summary>
        /// <param name="release">Объект релиза для загрузки</param>
        Task UploadReleaseAsync(Release release);

        /// <summary>
        /// Асинхронно получает релиз по идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор релиза</param>
        /// <returns>
        /// <para>
        /// <see cref="Release"/> - найденный релиз<br/>
        /// <see langword="null"/> - если релиз не существует
        /// </para>
        /// </returns>
        Task<Release?> GetReleaseAsync(int id);

        /// <summary>
        /// Асинхронно удаляет релиз из хранилища
        /// </summary>
        /// <param name="release">Объект релиза для удаления</param>
        Task RemoveReleaseAsync(Release release);
    }
}
