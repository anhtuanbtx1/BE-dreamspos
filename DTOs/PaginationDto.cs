namespace PosStore.DTOs
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc"; // asc or desc
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
