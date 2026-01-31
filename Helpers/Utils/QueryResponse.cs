namespace JobNexus.Helpers.Utils
{
    public class QueryResponse<T>
    {
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<T> Items { get; set; } = [];
    }
}
