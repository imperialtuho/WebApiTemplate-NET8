using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Domain.Common;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.Domain.Enums;
using WebApiTemplate.Domain.Exceptions;
using WebApiTemplate.Domain.Extensions;
using WebApiTemplate.Domain.SharedKernel;

namespace WebApiTemplate.Infrastructure.Repositories.Providers
{
    /// <summary>
    /// Entity framework generic repository.
    /// </summary>
    /// <typeparam name="C">DbContext type.</typeparam>
    /// <typeparam name="T">Entity type that extends BaseEntity.</typeparam>
    public abstract class EntityFrameworkGenericRepository<C, T> : IEntityFrameworkGenericRepository<T>
        where T : BaseEntity<string>
        where C : DbContext
    {
        protected C _dbContext;
        protected ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected const int DefaultTenantId = 0;

        private UserSession? _UserSession;

        /// <summary>
        /// Retrieves the tenant identifier from the HTTP context.
        /// </summary>
        private int? TenantIdentify => _httpContextAccessor.GetTenantIdentify() ?? DefaultTenantId;

        /// <summary>
        /// Gets the tenant ID from the logged-in session or HTTP context.
        /// </summary>
        public int? TenantId => LoginSession?.TenantId ?? TenantIdentify;

        /// <summary>
        /// Retrieves the user session from the HTTP context.
        /// </summary>
        public UserSession? LoginSession
        {
            get => _UserSession ?? _httpContextAccessor?.GetUserSession();
            set
            {
                _UserSession = value;
            }
        }

        /// <summary>
        /// Initializes the repository with a database context, SQL connection factory, and HTTP context accessor.
        /// </summary>
        /// <param name="options">Database context options.</param>
        /// <param name="sqlConnectionFactory">SQL connection factory.</param>
        /// <param name="httpContextAccessor">HTTP context accessor for retrieving tenant and user session.</param>
        /// <exception cref="InvalidOperationException">Throws when can't create DdContext.</exception>
        protected EntityFrameworkGenericRepository(DbContextOptions<C> options, ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _dbContext = Activator.CreateInstance(typeof(C), options) as C ?? throw new InvalidOperationException("Cannot create DbContext");
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Creates database context options based on the SQL connection factory and connection type.
        /// </summary>
        /// <param name="sqlConnectionFactory">The SQL connection factory.</param>
        /// <param name="connectionStringType">The type of database connection.</param>
        /// <returns>Configured DbContextOptions.</returns>
        protected internal static DbContextOptions<C> CreateDbContextOptions(ISqlConnectionFactory sqlConnectionFactory, ConnectionStringType connectionStringType)
        {
            sqlConnectionFactory.SetConnectionStringType(connectionStringType);
            (string? connectionString, ConnectionStringType dbType) = sqlConnectionFactory.GetConnectionStringAndDbType();
            var optionsBuilder = new DbContextOptionsBuilder<C>();

            if (!string.IsNullOrEmpty(connectionString))
            {
                switch (dbType)
                {
                    case ConnectionStringType.PostgresqlConnection:
                        optionsBuilder.UseNpgsql(connectionString);
                        break;

                    case ConnectionStringType.SqlServerConnection:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;

                    default:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;
                }
            }

            return optionsBuilder.Options;
        }

        /// <summary>
        /// Adds an entity and saves changes in the database.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        /// <returns>True if save is successful, otherwise false.</returns>
        public virtual async Task<bool> AddAndSaveChangesAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
            int result = await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return result > 0;
        }

        /// <summary>
        /// Adds an entity, saves changes, and returns the saved entity.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        /// <returns>The saved entity.</returns>
        public virtual async Task<T> AddWithSaveChangesAndReturnModelAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return entity;
        }

        /// <summary>
        /// Adds an entity to the database without saving immediately.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        public async Task AddAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
        }

        /// <summary>
        /// Adds multiple entities to the database without saving immediately.
        /// </summary>
        /// <param name="entities">Entities to add.</param>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                InitializeEntity(entity);
            }

            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        /// <summary>
        /// Commits all changes made in the database context.
        /// </summary>
        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">Entity ID.</param>
        /// <returns>The entity if found, otherwise throws <see cref="NotFoundException"/>.</returns>
        public virtual async Task<T> GetEntityByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id) ?? throw new NotFoundException($"{nameof(GetEntityByIdAsync)} of {nameof(T)} with {id} not found!");
        }

        /// <summary>
        /// Retrieves an entity by its ID, including all related navigation properties.
        /// </summary>
        /// <param name="id">Entity ID.</param>
        /// <returns>The entity with related data if found, otherwise throws <see cref="NotFoundException"/>.</returns>
        public virtual async Task<T> GetEntityWithRelationByIdAsync(object id)
        {
            T? entity = await _dbContext.Set<T>().IncludeAllNavigations(_dbContext).FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));

            return entity ?? throw new NotFoundException($"{nameof(GetEntityByIdAsync)} of {nameof(T)} with {id} not found!");
        }

        /// <summary>
        /// Updates an entity and saves changes in the database.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>True if update is successful, otherwise false.</returns>
        public async Task<bool> UpdateAndSaveChangesAsync(T entity)
        {
            UpdateEntity(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            int result = await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return result > 0;
        }

        /// <summary>
        /// Updates an entity, saves changes, and returns the updated entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>The updated entity.</returns>
        public async Task<T> UpdateWithSaveChangesAndReturnModelAsync(T entity)
        {
            UpdateEntity(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return entity;
        }

        /// <summary>
        /// Deletes an entity and saves changes in the database.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>True if delete is successful, otherwise false.</returns>
        public async Task<bool> DeleteAndSaveChangesAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Deletes multiple entities and saves changes in the database.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        /// <returns>True if delete is successful, otherwise false.</returns>
        public async Task<bool> DeleteRangeAndSaveChangesAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Deletes multiple entities without saving immediately.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        /// <summary>
        /// Searches for entities with pagination support.
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1).</param>
        /// <param name="pageSize">Page size (default: 10).</param>
        /// <param name="predicate">Optional filter condition.</param>
        /// <returns>Paginated list of entities.</returns>
        public async Task<PaginatedResponse<T>> SearchWithPaginatedResponseAsync(int pageNumber = 1, int pageSize = 10, Func<IQueryable<T>, IQueryable<T>>? predicate = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

            if (predicate != null)
            {
                query = predicate(query);
            }

            return await PaginatedResponse<T>.CreateAsync(query, pageNumber, pageSize);
        }

        /// <summary>
        /// Initializes an entity before saving.
        /// </summary>
        /// <param name="entity">Entity to initialize.</param>
        private void InitializeEntity(T entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.TenantId = TenantId;

            if (string.IsNullOrEmpty(entity.CreatedBy))
            {
                entity.CreatedBy = LoginSession?.Email ?? "Site Administrators";
            }

            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = null;
            entity.ModifiedBy = null;
            entity.IsDeleted = false;
        }

        /// <summary>
        /// Updates an entity's modified date and user information.
        /// </summary>
        private void UpdateEntity(T entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = LoginSession?.Email ?? "Site Administrators";
        }
    }
}