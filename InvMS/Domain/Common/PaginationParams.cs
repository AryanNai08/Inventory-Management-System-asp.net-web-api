namespace Domain.Common
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; } = "asc"; // "asc" or "desc"
    }
}
