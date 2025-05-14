using System.Linq.Expressions;
using WebApiTemplate.Domain.Common;

namespace WebApiTemplate.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines a generic repository for performing CRUD operations using Entity Framework.
    /// This interface provides methods for interacting with the database, including creating, reading, updating, and deleting entities.
    /// </summary>
    /// <typeparam name="T">The entity type that the repository will manage. The type must be a class that represents a database entity.</typeparam>
    /// <remarks>
    /// This interface is intended to be implemented by repositories that interact with an Entity Framework context.
    /// It abstracts away the complexity of managing the underlying database operations, providing a set of common methods for data manipulation.
    /// These methods can be extended or overridden in concrete repository classes to accommodate additional business logic or requirements.
    /// </remarks>
    public interface IEntityFrameworkGenericRepository<T> where T : class
    {
        /// <summary>
        /// Asynchronously determines whether any entities match the specified condition.
        /// </summary>
        /// <param name="predicate">A lambda expression to test each entity for a condition.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if any entities match the condition; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method checks the data source without retrieving full entities, making it efficient for existence checks or conditional logic.
        /// </remarks>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Asynchronously adds a new entity to the database context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method adds the entity to the context but does not immediately save it to the database. To persist changes,
        /// a commit operation must be called (e.g., <see cref="CommitAsync"/>).
        /// </remarks>
        Task AddAsync(T entity);

        /// <summary>
        /// Asynchronously adds a collection of entities to the database context.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method adds a collection of entities to the context but does not immediately save them to the database.
        /// A commit operation must be called to persist the changes (e.g., <see cref="CommitAsync"/>).
        /// </remarks>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Adds a new entity to the database and saves changes immediately.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task indicating whether the operation was successful (true if successful, false otherwise).</returns>
        /// <remarks>
        /// This method adds the entity to the context and immediately commits the changes to the database.
        /// </remarks>
        Task<bool> AddAndSaveChangesAsync(T entity);

        /// <summary>
        /// Adds a collection of entities to the database and saves changes immediately.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>
        /// A task indicating whether the operation was successful (true if successful, false otherwise).
        /// </returns>
        /// <remarks>
        /// This method adds multiple entities to the context and immediately commits the changes to the database.
        /// If any entity fails to be added, the operation may not be fully completed.
        /// </remarks>
        Task<bool> AddRangeAndSaveChangesAsync(IEnumerable<T> entities);

        /// <summary>
        /// Adds a new entity, saves changes immediately, and returns the saved entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task returning the added entity after committing the changes.</returns>
        /// <remarks>
        /// This method adds the entity to the context, saves it immediately, and returns the saved entity.
        /// Useful when you need to confirm the entity’s final state after saving.
        /// </remarks>
        Task<T> AddWithSaveChangesAndReturnModelAsync(T entity);

        /// <summary>
        /// Adds a collection of entities to the database, saves changes immediately, and returns the added entities.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>
        /// A task that resolves to a list of the added entities, reflecting any database-generated values.
        /// </returns>
        /// <remarks>
        /// This method adds multiple entities to the context, commits the changes to the database immediately,
        /// and returns the added entities, including any automatically generated fields such as IDs.
        /// </remarks>
        Task<IList<T>> AddRangeWithSaveChangesAndReturnModelsAsync(IEnumerable<T> entities);

        /// <summary>
        /// Commits all tracked changes to the database.
        /// </summary>
        /// <remarks>
        /// This method persists all changes made to entities in the context to the database. It does not affect any changes made
        /// to entities that are not tracked by the context. Once called, all modifications (insertions, updates, or deletions)
        /// that have been marked will be saved to the database.
        /// </remarks>
        void Commit();

        /// <summary>
        /// Commits any pending changes to the database asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous commit operation.</returns>
        /// <remarks>
        /// This method commits all changes made to entities in the current context to the database.
        /// All changes will be saved when this method is called.
        /// </remarks>
        Task CommitAsync();

        /// <summary>
        /// Soft deletes an entity and saves changes immediately.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>A task indicating whether the deletion was successful (true if successful, false otherwise).</returns>
        /// <remarks>
        /// This method marks the entity as deleted (e.g., setting a "IsDeleted" flag) instead of removing it from the database.
        /// Changes are immediately saved to the database.
        /// </remarks>
        Task<bool> SoftDeleteAndSaveChangesAsync(T entity);

        /// <summary>
        /// Soft deletes multiple entities without immediately saving changes.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <remarks>
        /// This method marks multiple entities as deleted but does not commit the changes to the database until explicitly committed.
        /// </remarks>
        void SoftDeleteRange(IEnumerable<T> entities);

        /// <summary>
        /// Soft deletes multiple entities and saves changes immediately.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <returns>A task indicating whether the deletion was successful (true if successful, false otherwise).</returns>
        /// <remarks>
        /// This method marks multiple entities as deleted and saves the changes immediately.
        /// </remarks>
        Task<bool> SoftDeleteRangeAndSaveChangesAsync(IEnumerable<T> entities);

        /// <summary>
        /// Soft deletes all entities that match a given condition in the database.
        /// </summary>
        /// <param name="predicate">A lambda expression specifying the condition for deletion.</param>
        /// <returns>A task indicating whether any records were deleted.</returns>
        /// <remarks>
        /// This method marks entities that match the condition as deleted but does not remove them from the database.
        /// </remarks>
        Task<bool> SoftDeleteWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Permanently deletes a single entity from the database.
        /// </summary>
        /// <param name="entity">The entity to be permanently deleted from the database.</param>
        /// <returns>
        /// Returns a <see cref="Task{Boolean}"/>. The task result indicates whether the entity was successfully deleted.
        /// True if the entity was deleted, otherwise false.
        /// </returns>
        /// <remarks>
        /// This method removes the specified entity from the database without any soft delete (i.e., it is permanently deleted).
        /// After calling this method, the entity will be physically removed from the database, and it cannot be restored unless there is
        /// an external backup or transactional support.
        /// </remarks>
        Task<bool> ForceDeleteAsync(T entity);

        /// <summary>
        /// Permanently deletes multiple entities from the database.
        /// </summary>
        /// <param name="entities">A collection of entities to be permanently deleted from the database.</param>
        /// <returns>
        /// Returns a <see cref="Task{Boolean}"/>. The task result indicates whether the operation was successful.
        /// True if all entities were successfully deleted, otherwise false.
        /// </returns>
        /// <remarks>
        /// This method removes all the specified entities from the database without any soft delete mechanism. The entities will be permanently deleted.
        /// This is useful when you need to remove multiple records at once, and you are certain that the deletion should be permanent.
        /// After calling this method, the entities will be physically removed from the database, and they cannot be restored unless backed up.
        /// </remarks>
        Task<bool> ForceDeleteRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Permanently deletes all entities that match the given condition from the database.
        /// </summary>
        /// <param name="predicate">A predicate expression defining the condition to match entities to delete.</param>
        /// <returns>
        /// Returns a <see cref="Task{Boolean}"/>. The task result indicates whether the deletion operation was successful.
        /// True if the matching entities were successfully deleted, otherwise false.
        /// </returns>
        /// <remarks>
        /// This method finds all entities in the database that match the specified condition (predicate) and permanently removes them.
        /// The entities are deleted without any soft delete operation (i.e., they are permanently deleted from the database).
        /// If no entities match the condition, the method will return false, indicating no entities were deleted.
        /// This method allows you to delete a dynamic set of records based on a condition provided at runtime.
        /// </remarks>
        Task<bool> ForceDeleteWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key value of the entity.</param>
        /// <returns>A task returning the entity if found, otherwise <c>null</c>.</returns>
        /// <remarks>
        /// This method retrieves an entity by its primary key from the database.
        /// </remarks>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        /// Retrieves an entity by its primary key, including related navigation properties.
        /// </summary>
        /// <param name="id">The primary key value of the entity.</param>
        /// <returns>A task returning the entity with its relations if found, otherwise <c>null</c>.</returns>
        /// <remarks>
        /// This method retrieves an entity by its primary key, including related entities (navigation properties).
        /// </remarks>
        Task<T> GetWithRelationByIdAsync(object id);

        /// <summary>
        /// Updates the specified entity in the database, marking it as modified.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <remarks>
        /// This method tracks the specified entity for update in the context. The entity should already exist in the database.
        /// The entity's fields will be modified based on the provided entity. Changes will not be immediately persisted to the database.
        /// Instead, the changes will be saved when the <see cref="Commit"/> synchronous method or the <see cref="CommitAsync"/> asynchronous method is called.
        /// </remarks>
        public void Update(T entity);

        /// <summary>
        /// Updates multiple entities in the database, marking them as modified.
        /// </summary>
        /// <param name="entities">The list of entities to update.</param>
        /// <remarks>
        /// This method tracks multiple entities for update in the context. The entities should already exist in the database.
        /// Their fields will be modified based on the provided entities. Changes will not be immediately persisted to the database.
        /// Instead, the changes will be saved when the <see cref="Commit"/> synchronous method or the <see cref="CommitAsync"/> asynchronous method is called.
        /// </remarks>
        public void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity and saves changes immediately.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task indicating whether the update was successful (true if successful, false otherwise).</returns>
        /// <remarks>
        /// This method updates an existing entity and commits the changes to the database.
        /// </remarks>
        Task<bool> UpdateAndSaveChangesAsync(T entity);

        /// <summary>
        /// Updates an existing entity, saves changes, and returns the updated entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task returning the updated entity after saving.</returns>
        /// <remarks>
        /// This method updates an existing entity, saves it, and returns the updated entity.
        /// </remarks>
        Task<T> UpdateWithSaveChangesAndReturnModelAsync(T entity);

        /// <summary>
        /// Searches for entities based on a filter and returns paginated results.
        /// </summary>
        /// <param name="pageNumber">The page number (default is 1).</param>
        /// <param name="pageSize">The number of records per page (default is 10).</param>
        /// <param name="predicate">An optional query filter.</param>
        /// <returns>A task returning a paginated response of entities.</returns>
        /// <remarks>
        /// This method allows you to filter and paginate results based on a given condition.
        /// </remarks>
        Task<PaginatedResponse<T>> SearchWithPaginatedResponseAsync(
            int pageNumber = 1,
            int pageSize = 10,
            Func<IQueryable<T>, IQueryable<T>>? predicate = null);

        /// <summary>
        /// Executes a paginated search operation against the database, applying dynamic filters
        /// based on the provided <see cref="SearchRequest"/>, and returns a <see cref="PaginatedResponse{T}"/> containing the filtered results.
        /// </summary>
        /// <param name="request">
        /// The <see cref="SearchRequest"/> object that contains the filtering criteria, page number, and page size for the search operation.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="PaginatedResponse{T}"/> object
        /// with the filtered and paginated list of entities of type <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        /// This method supports flexible, runtime-defined filtering and ensures efficient database querying
        /// by combining filtering and pagination before executing the query.
        /// </remarks>
        Task<PaginatedResponse<T>> SearchWithPaginatedResponseAsync(SearchRequest request);
    }
}