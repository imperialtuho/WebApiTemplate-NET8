namespace WebApiTemplate.Domain.Common
{
    public class SearchRequest
    {
        public string Keyword { get; set; } = string.Empty;

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public bool IsIncludingDelete { get; set; }
    }
}