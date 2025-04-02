using Microsoft.EntityFrameworkCore;

namespace WebApiTemplate.Domain.Common
{
    /// <summary>
    /// Represents a paginated response containing a subset of data and pagination metadata.
    /// </summary>
    /// <typeparam name="TResponse">The type of data being paginated.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="PaginatedResponse{TResponse}"/> class.
    /// </remarks>
    /// <param name="items">The collection of items for the current page.</param>
    /// <param name="count">The total number of items in the full data set.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    public class PaginatedResponse<TResponse>(IReadOnlyCollection<TResponse> items, int count, int pageNumber, int pageSize)
    {
        /// <summary>
        /// Gets the collection of paginated data items.
        /// </summary>
        public IReadOnlyCollection<TResponse> Data { get; } = items;

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int PageNumber { get; } = pageNumber;

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);

        /// <summary>
        /// Gets the total number of records in the data set.
        /// </summary>
        public int TotalCount { get; } = count;

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Asynchronously creates a paginated response from a queryable data source.
        /// </summary>
        /// <param name="source">The queryable data source containing the items to paginate.</param>
        /// <param name="pageNumber">The page number to retrieve, starting from 1.</param>
        /// <param name="pageSize">The number of items to include per page.</param>
        /// <returns>A task representing the asynchronous operation, with a <see cref="PaginatedResponse{TResponse}"/> as the result.</returns>
        /// <remarks>
        /// This method calculates the total number of items in the source, then retrieves the appropriate subset of items
        /// for the requested page using pagination logic. The result is wrapped in a <see cref="PaginatedResponse{TResponse}"/>
        /// object, which includes both the data and metadata such as the total number of pages, total count of items, and
        /// navigation properties for pagination (e.g., whether there are previous or next pages).
        /// </remarks>
        public static async Task<PaginatedResponse<TResponse>> CreateAsync(IQueryable<TResponse> source, int pageNumber, int pageSize)
        {
            int count = await source.CountAsync();
            List<TResponse> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResponse<TResponse>(items, count, pageNumber, pageSize);
        }
    }
}