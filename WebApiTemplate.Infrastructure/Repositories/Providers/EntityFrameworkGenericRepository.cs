using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
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
    /// Provides a generic repository implementation for Entity Framework, supporting CRUD operations with soft delete and multi-tenancy.
    /// This class is designed to interact with an Entity Framework database context to perform standard CRUD operations,
    /// and includes additional features such as soft delete (marking entities as deleted without physically removing them)
    /// and multi-tenancy support (filtering data based on tenant-specific criteria).
    /// </summary>
    /// <typeparam name="C">The type of the database context that will be used for operations. It must inherit from <see cref="DbContext"/>.</typeparam>
    /// <typeparam name="T">The type of the entity being managed by the repository. It must inherit from <see cref="BaseEntity{string}"/>.</typeparam>
    /// <remarks>
    /// This repository is abstract, allowing it to be extended by specific repositories that implement additional functionality
    /// as needed. The repository ensures that basic CRUD operations are implemented in a consistent manner, and it is optimized
    /// for working with soft deletes and multi-tenant environments.
    /// </remarks>
    public abstract class EntityFrameworkGenericRepository<C, T> : IEntityFrameworkGenericRepository<T>
        where T : BaseEntity<string>
        where C : DbContext
    {
        protected const int DefaultTenantId = 0;
        protected readonly C _dbContext;
        protected readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EntityFrameworkGenericRepository<C, T>> _logger;

        /// <summary>
        /// Retrieves the current user session associated with the active HTTP request.
        /// </summary>
        /// <value>
        /// Returns an instance of <see cref="UserSession"/> containing information about the authenticated user.
        /// </value>
        /// <remarks>
        /// This property provides access to user-specific session data extracted from the HTTP context,
        /// typically used for authentication, authorization, or auditing purposes.
        /// </remarks>
        protected UserSession LoginSession => _httpContextAccessor.GetUserSession();

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkGenericRepository{C, T}"/> class.
        /// </summary>
        /// <param name="options">The database context options.</param>
        /// <param name="sqlConnectionFactory">The SQL connection factory.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for retrieving user and tenant information.</param>
        /// <param name="logger">The logger instance for logging errors and activity.</param>
        /// <exception cref="InvalidOperationException">Thrown if the DbContext cannot be created.</exception>
        protected EntityFrameworkGenericRepository(
            DbContextOptions<C> options,
            ISqlConnectionFactory sqlConnectionFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EntityFrameworkGenericRepository<C, T>> logger)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _dbContext = Activator.CreateInstance(typeof(C), options) as C ?? throw new InvalidOperationException("Cannot create DbContext");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously adds a new entity to the database context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method adds the entity to the context but does not immediately save it to the database.
        /// To persist changes, a commit operation must be called (e.g., <see cref="CommitAsync"/>).
        /// </remarks>
        public async Task AddAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
        }

        /// <summary>
        /// Asynchronously adds multiple entities to the database context.
        /// </summary>
        /// <param name="entities">The list of entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method adds multiple entities to the context but does not immediately save them to the database.
        /// To persist changes, a commit operation must be called (e.g., <see cref="CommitAsync"/>).
        /// </remarks>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                InitializeEntity(entity);
            }

            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        /// <summary>
        /// Asynchronously adds a new entity to the database context, saves changes to the database, and returns the saved entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity, which has been saved to the database.</returns>
        /// <remarks>
        /// This method adds the entity to the context, immediately commits the changes to the database,
        /// and returns the saved entity. The returned entity will have any autogenerated values (e.g., ID) updated.
        /// </remarks>, otherwise false.</returns>
        public virtual async Task<bool> AddAndSaveChangesAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
            int result = await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return result > 0;
        }

        /// <summary>
        /// Adds a collection of entities to the database and saves changes immediately.
        /// </summary>
        /// <param name="entities">The collection of entities to be added.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is <c>true</c> if at least one entity was successfully added;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method inserts multiple entities into the database context and commits the changes immediately.
        /// If the save operation fails, it may result in a partial or unsuccessful transaction.
        /// After saving, each entity's state is set to <see cref="EntityState.Unchanged"/> to prevent unintended modifications.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the provided collection is null or empty, as at least one entity is required.
        /// </exception>
        public async Task<bool> AddRangeAndSaveChangesAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
            {
                throw new InvalidOperationException($"{nameof(entities)} cannot be null or empty; at least one entity is required.");
            }

            foreach (T entity in entities)
            {
                InitializeEntity(entity);
            }

            await _dbContext.Set<T>().AddRangeAsync(entities);
            int result = await _dbContext.SaveChangesAsync();

            foreach (T entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Unchanged;
            }

            return result > 0;
        }

        /// <summary>
        /// Asynchronously adds a new entity to the database context, saves changes to the database, and returns the saved entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity, which has been saved to the database.</returns>
        /// <remarks>
        /// This method adds the entity to the context, immediately commits the changes to the database,
        /// and returns the saved entity. The returned entity will have any autogenerated values (e.g., ID) updated.
        /// </remarks>
        public virtual async Task<T> AddWithSaveChangesAndReturnModelAsync(T entity)
        {
            InitializeEntity(entity);
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return entity;
        }

        /// <summary>
        /// Adds a collection of entities to the database, commits the changes immediately, and returns the added entities.
        /// </summary>
        /// <param name="entities">The collection of entities to be added.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a list of the added entities,
        /// including any database-generated values such as primary keys.
        /// </returns>
        /// <remarks>
        /// This method inserts multiple entities into the database context and immediately saves the changes.
        /// After saving, the entities are returned with any automatically generated fields (e.g., primary keys) populated.
        /// Each entity's state is set to <see cref="EntityState.Unchanged"/> to prevent unintended modifications after saving.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the provided collection is null or empty, as at least one entity is required.
        /// </exception>
        public async Task<IList<T>> AddRangeWithSaveChangesAndReturnModelsAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
            {
                throw new InvalidOperationException($"{nameof(entities)} cannot be null or empty; at least one entity is required.");
            }

            foreach (T entity in entities)
            {
                InitializeEntity(entity);
            }

            await _dbContext.Set<T>().AddRangeAsync(entities);
            await SaveChangesAsync();

            foreach (T entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Unchanged;
            }

            return entities as IList<T> ?? entities.ToList();
        }

        /// <summary>
        /// Commits all tracked changes to the database.
        /// </summary>
        /// <remarks>
        /// This method persists all changes made to entities in the context to the database. It does not affect any changes made
        /// to entities that are not tracked by the context. Once called, all modifications (insertions, updates, or deletions)
        /// that have been marked will be saved to the database.
        /// </remarks>
        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Commits all changes made in the database context by saving them to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method saves all changes made to entities in the context to the database.
        /// If changes have been added to entities but not yet saved, this method will persist them.
        /// </remarks>
        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Soft deletes an entity by marking it as deleted, and saves the changes to the database.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        /// <returns>True if the entity was successfully soft deleted and changes were saved; otherwise, false.</returns>
        /// <remarks>
        /// This method marks the entity as deleted by setting a flag in the database (e.g., setting a "IsDeleted" property).
        /// The entity is not removed from the database, but it is excluded from active queries.
        /// </remarks>
        public async Task<bool> SoftDeleteAndSaveChangesAsync(T entity)
        {
            SoftDeleteEntity(entity);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// Soft deletes multiple entities by marking each as deleted without saving changes immediately.
        /// </summary>
        /// <param name="entities">The list of entities to soft delete.</param>
        /// <remarks>
        /// This method marks the specified entities as deleted by setting a flag in the database for each entity.
        /// Changes are not saved immediately; a commit operation must be called to persist the changes.
        /// </remarks>
        public void SoftDeleteRange(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                SoftDeleteEntity(entity);
            }
        }

        /// <summary>
        /// Soft deletes multiple entities by marking each as deleted and saves the changes to the database.
        /// </summary>
        /// <param name="entities">The list of entities to soft delete.</param>
        /// <returns>True if the entities were successfully soft deleted and changes were saved; otherwise, false.</returns>
        /// <remarks>
        /// This method marks the specified entities as deleted by setting a flag in the database (e.g., setting a "IsDeleted" property).
        /// The changes are immediately committed to the database, and the entities are excluded from active queries.
        /// </remarks>
        public async Task<bool> SoftDeleteRangeAndSaveChangesAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                SoftDeleteEntity(entity);
            }

            return await SaveChangesAsync();
        }

        /// <summary>
        /// Soft deletes all entities matching a given predicate and saves changes to the database.
        /// </summary>
        /// <param name="predicate">The condition that must be met for an entity to be soft deleted.</param>
        /// <returns>True if the matching entities were successfully soft deleted and changes were saved; otherwise, false.</returns>
        /// <remarks>
        /// This method marks all entities that satisfy the given predicate as deleted by setting a flag in the database.
        /// Changes are immediately committed to the database.
        /// </remarks>
        public async Task<bool> SoftDeleteWhereAsync(Expression<Func<T, bool>> predicate)
        {
            List<T>? entities = await _dbContext.Set<T>().Where(predicate).ToListAsync();

            foreach (T entity in entities)
            {
                SoftDeleteEntity(entity);
            }

            return await SaveChangesAsync();
        }

        /// <summary>
        /// Forces the deletion of an entity (permanently removes it from the database).
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>True if delete is successful, otherwise false.</returns>
        /// <remarks>
        /// This method removes the entity permanently from the database, bypassing the soft delete mechanism.
        /// </remarks>
        public async Task<bool> ForceDeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);

            return await SaveChangesAsync();
        }

        /// <summary>
        /// Forces the deletion of multiple entities (permanently removes them from the database).
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        /// <returns>True if delete is successful, otherwise false.</returns>
        /// <remarks>
        /// This method removes multiple entities permanently from the database, bypassing the soft delete mechanism.
        /// </remarks>
        public async Task<bool> ForceDeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// Forces the deletion of entities matching a given predicate (permanently removes them from the database).
        /// </summary>
        /// <param name="predicate">The condition to filter entities for deletion.</param>
        /// <returns>True if delete is successful, otherwise false.</returns>
        /// <remarks>
        /// This method removes the entities that satisfy the given predicate permanently from the database, bypassing the soft delete mechanism.
        /// </remarks>
        public async Task<bool> ForceDeleteWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ExecuteDeleteAsync() > 0;
        }

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">Entity ID.</param>
        /// <returns>The entity if found, otherwise throws <see cref="NotFoundException"/>.</returns>
        /// <remarks>
        /// This method retrieves the entity from the database by its ID.
        /// If the entity is not found, it throws a <see cref="NotFoundException"/>.
        /// </remarks>
        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id) ?? throw new NotFoundException($"{nameof(GetByIdAsync)} of {nameof(T)} with {id} not found!");
        }

        /// <summary>
        /// Retrieves an entity by its ID, including all related navigation properties.
        /// </summary>
        /// <param name="id">Entity ID.</param>
        /// <returns>The entity with related data if found, otherwise throws <see cref="NotFoundException"/>.</returns>
        /// <remarks>
        /// This method retrieves the entity from the database by its ID, along with any related navigation properties.
        /// If the entity is not found, it throws a <see cref="NotFoundException"/>.
        /// </remarks>
        public virtual async Task<T> GetWithRelationByIdAsync(object id)
        {
            T? entity = await _dbContext.Set<T>().IncludeAllNavigations(_dbContext).FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));

            return entity ?? throw new NotFoundException($"{nameof(GetByIdAsync)} of {nameof(T)} with {id} not found!");
        }

        /// <summary>
        /// Updates the specified entity in the database, marking it as modified.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <remarks>
        /// This method tracks the specified entity for update in the context. The entity should already exist in the database.
        /// The entity's fields will be modified based on the provided entity. The changes will not be immediately persisted to the database.
        /// The changes will only be saved when the <see cref="CommitAsync"/> method is called.
        /// </remarks>
        public void Update(T entity)
        {
            // Update entity properties (e.g., ModifiedDate, ModifiedBy) before marking it as modified.
            UpdateEntity(entity);

            // Mark the entity as modified (without saving immediately).
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Updates multiple entities in the database, marking them as modified.
        /// </summary>
        /// <param name="entities">The list of entities to update.</param>
        /// <remarks>
        /// This method tracks multiple entities for update in the context. The entities should already exist in the database.
        /// Their fields will be modified based on the provided entities. The changes will not be immediately persisted to the database.
        /// The changes will only be saved when the <see cref="CommitAsync"/> method is called.
        /// </remarks>
        public void UpdateRange(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                // Update entity properties (e.g., ModifiedDate, ModifiedBy) before marking it as modified.
                UpdateEntity(entity);

                // Mark each entity as modified (without saving immediately).
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// Updates an entity and saves changes in the database.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>True if update is successful, otherwise false.</returns>
        /// <remarks>
        /// This method updates the entity in the database and commits the changes.
        /// If the operation is successful, it returns true; otherwise, false.
        /// </remarks>
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
        /// <remarks>
        /// This method updates the entity in the database, commits the changes, and returns the updated entity.
        /// </remarks>
        public async Task<T> UpdateWithSaveChangesAndReturnModelAsync(T entity)
        {
            UpdateEntity(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;

            return entity;
        }

        /// <summary>
        /// Searches for entities with pagination support.
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1).</param>
        /// <param name="pageSize">Page size (default: 10).</param>
        /// <param name="predicate">Optional filter condition.</param>
        /// <returns>Paginated list of entities.</returns>
        /// <remarks>
        /// This method retrieves a paginated list of entities from the database, with optional filtering based on a predicate.
        /// </remarks>
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
        /// Creates database context options based on the SQL connection factory and connection type.
        /// </summary>
        /// <param name="sqlConnectionFactory">The SQL connection factory.</param>
        /// <param name="connectionStringType">The type of database connection.</param>
        /// <returns>Configured DbContextOptions.</returns>
        /// <remarks>
        /// This method configures and creates the necessary DbContextOptions based on the connection string and the database type (e.g., SQL Server, PostgreSQL).
        /// </remarks>
        protected internal static DbContextOptions<C> CreateDbContextOptions(ISqlConnectionFactory sqlConnectionFactory, ConnectionStringType connectionStringType)
        {
            sqlConnectionFactory.SetConnectionStringType(connectionStringType);
            (string? connectionString, ConnectionStringType dbType) = sqlConnectionFactory.GetConnectionStringAndDbType();
            var optionsBuilder = new DbContextOptionsBuilder<C>();

            if (!string.IsNullOrEmpty(connectionString))
            {
                switch (dbType)
                {
                    case ConnectionStringType.PostgreSqlConnection:
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
        /// Initializes an entity before saving.
        /// </summary>
        /// <param name="entity">Entity to initialize.</param>
        /// <remarks>
        /// This method initializes the entity by setting properties like `Id`, `TenantId`, `CreatedBy`, and `CreatedDate`.
        /// It also sets the entity to a non-deleted state.
        /// </remarks>
        private void InitializeEntity(T entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.TenantId = LoginSession.TenantId;

            if (string.IsNullOrEmpty(entity.CreatedBy))
            {
                entity.CreatedBy = LoginSession.Email;
            }

            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = LoginSession.Email;
            entity.IsDeleted = false;
        }

        /// <summary>
        /// Updates an entity's modified date and user information.
        /// </summary>
        /// <remarks>
        /// This method updates the `ModifiedDate` and `ModifiedBy` properties of the entity to reflect the most recent changes.
        /// </remarks>
        private void UpdateEntity(T entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = LoginSession.Email;
        }

        /// <summary>
        /// Marks an entity as deleted and updates its modification details.
        /// </summary>
        /// <param name="entity">Entity to soft delete.</param>
        /// <remarks>
        /// This method sets the `IsDeleted` flag of the entity to true, along with the modification date and user.
        /// </remarks>
        private void SoftDeleteEntity(T entity)
        {
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = LoginSession.Email;
        }

        /// <summary>
        /// Saves the changes in the database context.
        /// </summary>
        /// <returns>True if save is successful, otherwise false.</returns>
        /// <remarks>
        /// This method commits the changes made in the database context to the database.
        /// </remarks>
        private async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes in {Repository}", typeof(T).Name);

                return false;
            }
        }
    }
}