namespace DisneyApi.Models {
    public class PagedResult<T>
    {
        public List<T> Items {get; set;} = [];
        public int PageNumber {get; set;}
        public int PageSize {get; set;}
        public int TotalItems {get; set;}
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPrevPage => PageNumber > 1;
    }
}

