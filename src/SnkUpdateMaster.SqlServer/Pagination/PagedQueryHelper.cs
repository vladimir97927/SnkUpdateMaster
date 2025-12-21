namespace SnkUpdateMaster.SqlServer.Pagination
{
    internal static class PagedQueryHelper
    {
        public const string Offset = "Offset";

        public const string Next = "Next";

        public static PageData GetPageData(int? page, int? pageSize)
        {
            int offset;
            if (!page.HasValue || !pageSize.HasValue)
            {
                offset = 0;
            }
            else
            {
                offset = (page.Value - 1) * pageSize.Value;
            }

            int next;
            if (!pageSize.HasValue)
            {
                next = int.MaxValue;
            }
            else
            {
                next = pageSize.Value;
            }

            return new PageData(offset, next);
        }

        public static string AppendPageStatement(string sql)
        {
            return $"{sql} " +
                $"OFFSET @{Offset} ROWS FETCH NEXT @{Next} ROWS ONLY; ";
        }
    }
}
