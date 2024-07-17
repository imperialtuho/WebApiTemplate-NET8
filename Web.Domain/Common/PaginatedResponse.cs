using Microsoft.EntityFrameworkCore;

namespace Web.Domain.Common
{
    public class PaginatedResponse<TResponse>
    {
        public IReadOnlyCollection<TResponse> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public PaginatedResponse(IReadOnlyCollection<TResponse> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public static async Task<PaginatedResponse<TResponse>> CreateAsync(IQueryable<TResponse> source, int pageNumber, int pageSize)
        {
            int count = await source.CountAsync();
            List<TResponse> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResponse<TResponse>(items, count, pageNumber, pageSize);
        }
    }
}