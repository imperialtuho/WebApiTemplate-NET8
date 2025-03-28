using Microsoft.AspNetCore.Http;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.Infrastructure.Configurations;
using WebApiTemplate.Infrastructure.Database;

namespace WebApiTemplate.Infrastructure.Repositories.Providers.Example
{
    /// <summary>
    /// Repository for managing <see cref="ExampleEntity"/> instances.
    /// </summary>
    public class ExampleRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, ExampleEntity>, IExampleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleRepository"/> class.
        /// </summary>
        /// <param name="sqlConnectionFactory">The factory for creating database connections.</param>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        public ExampleRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
        }

        /// <summary>
        /// Retrieves an <see cref="ExampleEntity"/> by its identifier.
        /// If the provided <paramref name="id"/> is null or empty, a mock entity is returned.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the <see cref="ExampleEntity"/>.</returns>
        public async Task<ExampleEntity> GetExampleByIdAsync(string id)
        {
            bool isMocking = string.IsNullOrWhiteSpace(id);

            if (isMocking)
            {
                return new ExampleEntity() { Name = "This is example!" };
            }

            return await GetEntityByIdAsync(id);
        }
    }
}