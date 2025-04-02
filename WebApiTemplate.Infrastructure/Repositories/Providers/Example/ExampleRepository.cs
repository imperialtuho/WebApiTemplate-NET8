using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.Infrastructure.Configurations;
using WebApiTemplate.Infrastructure.Database;

namespace WebApiTemplate.Infrastructure.Repositories.Providers.Example
{
    /// <summary>
    /// Repository for managing <see cref="ExampleEntity"/> instances.
    /// This repository provides methods for interacting with the <see cref="ExampleEntity"/> entities in the database.
    /// It supports retrieving entities by their identifiers, and for missing identifiers, it can return mock data.
    /// </summary>
    public class ExampleRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, ExampleEntity>, IExampleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleRepository"/> class.
        /// This constructor sets up the repository with the necessary dependencies, including the database connection factory,
        /// the HTTP context accessor, and the logger for logging operations related to this repository.
        /// </summary>
        /// <param name="sqlConnectionFactory">The factory for creating database connections.</param>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <param name="logger">The logger instance used for logging repository operations.</param>
        /// <remarks>
        /// The constructor ensures that the repository has the required dependencies for performing operations on the
        /// database context and managing the HTTP context.
        /// </remarks>
        public ExampleRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor, ILogger<ExampleRepository> logger)
            : base(sqlConnectionFactory, httpContextAccessor, logger)
        {
        }

        /// <summary>
        /// Retrieves an <see cref="ExampleEntity"/> by its identifier.
        /// If the provided <paramref name="id"/> is null or empty, a mock entity is returned.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the <see cref="ExampleEntity"/>.</returns>
        /// <remarks>
        /// This method allows for the retrieval of an entity from the database by its unique identifier. If the identifier
        /// is null or empty, a mock instance of <see cref="ExampleEntity"/> is returned with a new GUID and a default name.
        /// If a valid identifier is provided, the method fetches the entity from the database using the base class's
        /// <see cref="GetByIdAsync"/> method.
        /// </remarks>
        public async Task<ExampleEntity> GetExampleByIdAsync(string id)
        {
            bool isMocking = string.IsNullOrWhiteSpace(id);

            if (isMocking)
            {
                return new ExampleEntity() { Id = Guid.NewGuid().ToString(), Name = "This is example!" };
            }

            return await GetByIdAsync(id);
        }
    }
}