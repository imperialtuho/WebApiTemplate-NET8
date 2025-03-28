using Microsoft.EntityFrameworkCore;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Domain.Common;
using WebApiTemplate.Domain.Exceptions;

namespace WebApiTemplate.Infrastructure.Repositories.Providers
{
    /// <summary>
    /// Provides a generic Entity Framework repository implementation for CRUD operations and paginated queries.
    /// </summary>
    /// <typeparam name="C">The type of the DbContext.</typeparam>
    /// <typeparam name="T">The entity type.</typeparam>
    public abstract class EFGenericRepository<C, T> : IEFGenericRepository<T>
        where T : class
        where C : DbContext
    {
        /// <summary>
        /// The database context instance.
        /// </summary>
        protected C _dbContext;

        /// <summary>
        /// The SQL connection factory used for obtaining raw database connections.
        /// </summary>
        protected ISqlConnectionFactory _sqlConnectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EFGenericRepository{C, T}"/> class.
        /// </summary>
        /// <param name="options">The options to configure the DbContext.</param>
        /// <param name="sqlConnectionFactory">The SQL connection factory.</param>
        /// <exception cref="InvalidOperationException">Thrown if the DbContext cannot be created.</exception>
        protected EFGenericRepository(DbContextOptions<C> options, ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _dbContext = Activator.CreateInstance(typeof(C), options) as C
                ?? throw new InvalidOperationException("Cannot create DbContext");
        }

        /// <summary>
        /// Adds a new entity to the context and saves changes immediately.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual async Task AddAndSaveChangesAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;
        }

        /// <summary>
        /// Adds a new entity to the context without saving changes immediately.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        /// <summary>
        /// Adds multiple entities to the context without saving changes immediately.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        /// <summary>
        /// Commits all changes made in the context to the database.
        /// </summary>
        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>
        /// The entity if found.
        /// </returns>
        /// <exception cref="NotFoundException">
        /// Thrown if the entity with the specified identifier is not found.
        /// </exception>
        public virtual async Task<T> GetEntityByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id)
                ?? throw new NotFoundException($"{nameof(GetEntityByIdAsync)} of {nameof(T)} with {id} not found!");
        }

        /// <summary>
        /// Updates an existing entity in the context and saves changes immediately.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public async Task UpdateAndSaveChangesAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;
        }

        /// <summary>
        /// Deletes an entity from the context and saves changes immediately.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public async Task DeleteAndSaveChangesAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes multiple entities from the context and saves changes immediately.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public async Task DeleteRangeAndSaveChangesAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes multiple entities from the context without saving changes immediately.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        /// <summary>
        /// Searches entities with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of items per page (default is 10).</param>
        /// <param name="predicate">
        /// An optional function to filter or modify the query.
        /// </param>
        /// <returns>
        /// A <see cref="PaginatedResponse{T}"/> containing the paginated results.
        /// </returns>
        public async Task<PaginatedResponse<T>> SearchWithPaginatedResponseAsync(
            int pageNumber = 1,
            int pageSize = 10,
            Func<IQueryable<T>, IQueryable<T>>? predicate = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

            if (predicate != null)
            {
                query = predicate(query);
            }

            return await PaginatedResponse<T>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}