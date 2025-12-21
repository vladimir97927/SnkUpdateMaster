using SnkUpdateMaster.Core.Common;

namespace SnkUpdateMaster.Core.ReleasePublisher
{
    /// <summary>
    /// Интерфейс для получения информации о релизах с поддержкой пагинации
    /// </summary>
    public interface IReleaseInfoSource
    {
        /// <summary>
        /// Асинхронно получает постраничный список информации о релизах
        /// </summary>
        /// <param name="page">Номер страницы (начиная с 1). null - без пагинации</param>
        /// <param name="pageSize">Количество элементов на странице. null - без пагинации</param>
        /// <returns>
        /// Пагинированные данные с информацией о релизах.
        /// <para>
        /// Для непагинированных запросов (<paramref name="page"/> = null) возвращает все элементы
        /// </para>
        /// </returns>
        Task<PagedData<IEnumerable<ReleaseInfo>>> GetReleaseInfosPagedAsync(int? page = null, int? pageSize = null);
    }
}
