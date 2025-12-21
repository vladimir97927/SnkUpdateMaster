namespace SnkUpdateMaster.Core.Common
{
    /// <summary>
    /// Класс постраничных данных с метаинформацией о пагинации
    /// </summary>
    /// <typeparam name="T">Тип элементов данных</typeparam>
    /// <remarks>
    /// Используется для возврата данных с дополнительной информацией:
    /// <list type="bullet">
    /// <item><description>Элементы текущей страницы</description></item>
    /// <item><description>Параметры пагинации</description></item>
    /// <item><description>Общее количество элементов</description></item>
    /// </list>
    /// </remarks>
    public class PagedData<T>
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="data">Данные на текущей странице</param>
        /// <param name="pageNumber">
        /// Номер страницы (начиная с 1). 
        /// null - если пагинация не применялась
        /// </param>
        /// <param name="pageSize">
        /// Количество элементов на странице. 
        /// null - если пагинация не применялась
        /// </param>
        /// <param name="totalCount">Общее число элементов на всех страницах</param>
        public PagedData(T data, int? pageNumber, int? pageSize, int totalCount)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Данные на текущей странице
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Номер текущей страницы. 
        /// null - если пагинация не применялась
        /// </summary>
        public int? PageNumber { get; }

        /// <summary>
        /// Количество элементов на странице.
        /// null - если пагинация не применялась
        /// </summary>
        public int? PageSize { get; }


        /// <summary>
        /// Общее число элементов на всех страницах
        /// </summary>
        public int TotalCount { get; }
    }
}
