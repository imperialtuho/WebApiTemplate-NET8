using WebApiTemplate.Domain.Common;

namespace WebApiTemplate.Application.Interfaces.Repositories
{
    public interface IEntityFrameworkGenericRepository<T> where T : class
    {
        Task<T> GetEntityByIdAsync(object id);

        Task<T> GetEntityWithRelationByIdAsync(object id);

        Task AddAsync(T entity);

        Task<bool> AddAndSaveChangesAsync(T entity);

        Task<T> AddWithSaveChangesAndReturnModelAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task<bool> UpdateAndSaveChangesAsync(T entity);

        Task<T> UpdateWithSaveChangesAndReturnModelAsync(T entity);

        Task<bool> DeleteAndSaveChangesAsync(T entity);

        Task<bool> DeleteRangeAndSaveChangesAsync(IEnumerable<T> entities);

        void DeleteRange(IEnumerable<T> entities);

        Task CommitAsync();

        Task<PaginatedResponse<T>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<T>, IQueryable<T>>? predicate = null);
    }
}