namespace SnkUpdateMaster.Core.UpdateSource
{
    /// <summary>
    /// Определяет интерфейс для получения информации об обновлениях из внешнего источника.
    /// </summary>
    /// <remarks>
    /// Интерфейс предоставляет абстракцию для различных источников обновлений:
    /// <list type="bullet">
    /// <item><description>Удаленный сервер обновлений</description></item>
    /// <item><description>Локальная файловая система</description></item>
    /// <item><description>База данных</description></item>
    /// </list>
    /// Реализации должны обрабатывать ошибки связи и форматирования данных
    /// </remarks>
    public interface IUpdateInfoProvider
    {
        /// <summary>
        /// Асинхронно получает информацию о последнем доступном обновлении
        /// </summary>
        /// <returns>
        /// При успешном выполнении содержит:
        /// <para>
        /// <see cref="UpdateInfo"/> - данные последнего обновления<br/>
        /// <see langword="null"/> - если обновления отсутствуют
        /// </para>
        /// </returns>
        Task<UpdateInfo?> GetLastUpdatesAsync(CancellationToken cancellationToken = default);
    }
}
