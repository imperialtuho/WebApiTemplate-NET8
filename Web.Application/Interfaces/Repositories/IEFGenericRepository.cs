using Web.Domain.Common;

namespace Web.Application.Interfaces.Repositories
{
    public interface IEFGenericRepository<T> where T : class
    {
        Task<T> GetEntityByIdAsync(object id);

        Task AddAsync(T entity);

        Task AddAndSaveChangesAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task UpdateAndSaveChangesAsync(T entity);

        Task DeleteAndSaveChangesAsync(T entity);

        Task DeleteRangeAndSaveChangesAsync(IEnumerable<T> entities);

        void DeleteRange(IEnumerable<T> entities);

        Task CommitAsync();

        Task<PaginatedResponse<T>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<T>, IQueryable<T>>? predicate = null);
    }
}