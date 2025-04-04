namespace WebApiTemplate.Domain.Common
{
    /// <summary>
    /// Represents a request for searching and paginating a collection of items, with optional filters.
    /// </summary>
    /// <remarks>
    /// This class is used to specify the filters, pagination settings (page number and page size),
    /// that can be applied when querying a collection of items.
    /// </remarks>
    public class SearchRequest
    {
        /// <summary>
        /// Gets or sets the list of filters to apply to the query.
        /// </summary>
        /// <value>A list of <see cref="FilterCriteria"/> objects representing the filter conditions.
        /// If not provided, no filters are applied.</value>
        public List<FilterCriteria>? Filters { get; set; }

        /// <summary>
        /// Gets or sets the page number to retrieve (starting from 1).
        /// </summary>
        /// <value>The page number for pagination. Defaults to 1 if not specified.
        /// A value of 1 indicates the first page.</value>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items to include per page.
        /// </summary>
        /// <value>The number of items to retrieve per page. Defaults to 10 if not specified.
        /// This value must be greater than 0 to apply pagination.</value>
        public int PageSize { get; set; } = 10;
    }
}