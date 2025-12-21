namespace SnkUpdateMaster.SqlServer.Pagination
{
    internal struct PageData
    {
        public PageData(int offset, int next)
        {
            Offset = offset;
            Next = next;
        }

        public int Offset { get; }

        public int Next { get; }
    }
}
