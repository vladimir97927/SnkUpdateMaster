namespace SnkUpdateMaster.Core.Common
{
    public class PagedData<T>
    {
        public PagedData(T data, int? pageNumber, int? pageSize, int totalCount)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public T Data { get; }

        public int? PageNumber { get; }

        public int? PageSize { get; }

        public int TotalCount { get; }
    }
}
