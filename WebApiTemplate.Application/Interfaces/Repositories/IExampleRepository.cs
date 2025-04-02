using WebApiTemplate.Domain.Entities;

namespace WebApiTemplate.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines the contract for a repository that handles operations related to <see cref="ExampleEntity"/>.
    /// This interface inherits from <see cref="IEntityFrameworkGenericRepository{ExampleEntity}"/> and extends it with
    /// additional methods specific to the <see cref="ExampleEntity"/> entity.
    /// </summary>
    /// <remarks>
    /// This interface includes the necessary CRUD operations for managing <see cref="ExampleEntity"/> entities
    /// in the database, as well as any domain-specific methods required for working with <see cref="ExampleEntity"/>.
    /// </remarks>
    public interface IExampleRepository : IEntityFrameworkGenericRepository<ExampleEntity>
    {
        /// <summary>
        /// Asynchronously retrieves an <see cref="ExampleEntity"/> by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the <see cref="ExampleEntity"/>.</param>
        /// <returns>A task representing the asynchronous operation, with the result containing the <see cref="ExampleEntity"/>.</returns>
        /// <remarks>
        /// If the <paramref name="id"/> is invalid (e.g., null or empty), the method may return a mock entity
        /// instead of querying the database, depending on the implementation.
        /// </remarks>
        Task<ExampleEntity> GetExampleByIdAsync(string id);
    }
}