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
        /// <typeparam name="TResponse">The type of the response items.</typeparam>
        /// <param name="source">
        /// The queryable data source containing the items to paginate.
        /// <para><b>Warning:</b> This method should only be used with an Entity Framework Core queryable source.</para>
        /// </param>
        /// <param name="pageNumber">The page number to retrieve, starting from 1.</param>
        /// <param name="pageSize">The number of items to include per page.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a <see cref="PaginatedResponse{TResponse}"/> as the result.
        /// </returns>
        /// <remarks>
        /// This method executes database queries asynchronously using Entity Framework Core's <c>CountAsync()</c> 
        /// and <c>ToListAsync()</c>. It does not support in-memory collections (e.g., <c>List{T}.AsQueryable()</c>).
        /// </remarks>
        public static async Task<PaginatedResponse<TResponse>> CreateAsync(IQueryable<TResponse> source, int pageNumber, int pageSize)
        {
            int count = await source.CountAsync();
            List<TResponse> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResponse<TResponse>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Creates a paginated response from the given <see cref="IQueryable{TResponse}"/> source.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response items.</typeparam>
        /// <param name="source">The queryable data source to paginate.</param>
        /// <param name="pageNumber">The current page number (1-based index).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>
        /// A <see cref="PaginatedResponse{TResponse}"/> containing the paginated items, total item count, and pagination metadata.
        /// </returns>
        /// <remarks>
        /// This method retrieves a subset of data from the source collection based on the specified pagination parameters.
        /// It calculates the total number of items and applies the appropriate <c>Skip()</c> and <c>Take()</c> operations to 
        /// extract the requested page.
        /// </remarks>
        public static PaginatedResponse<TResponse> Create(IQueryable<TResponse> source, int pageNumber, int pageSize)
        {
            int count = source.Count();
            List<TResponse> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedResponse<TResponse>(items, count, pageNumber, pageSize);
        }
    }
}