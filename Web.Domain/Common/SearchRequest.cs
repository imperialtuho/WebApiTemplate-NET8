namespace Web.Domain.Common
{
    public class SearchRequest
    {
        public string? Keyword { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}