using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.Domain.Enums;
using WebApiTemplate.Infrastructure.Repositories.Providers;

namespace WebApiTemplate.Infrastructure.Configurations
{
    /// <summary>
    /// A base repository that provides Entity Framework-based database operations
    /// for PostgreSQL connections, inheriting from the generic <see cref="EntityFrameworkGenericRepository{C, T}"/>.
    /// </summary>
    /// <typeparam name="C">The type of the database context, which must inherit from <see cref="DbContext"/>.</typeparam>
    /// <typeparam name="T">The type of the entity, which must inherit from <see cref="BaseEntity{string}"/>.</typeparam>
    /// <remarks>
    /// This class acts as a base class for repositories that interact with a PostgreSQL database, using Entity Framework
    /// for data access operations. It provides the necessary setup for managing database connections, performing CRUD operations,
    /// and interacting with the HTTP context, with PostgreSQL-specific configuration.
    /// </remarks>
    public abstract class DbPostgeSqlConnectionEFRepositoryBase<C, T> : EntityFrameworkGenericRepository<C, T>
        where T : BaseEntity<string>
        where C : DbContext, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbPostgeSqlConnectionEFRepositoryBase{C, T}"/> class.
        /// </summary>
        /// <param name="sqlConnectionFactory">The SQL connection factory used to configure the database connection.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor that provides access to the HTTP context.</param>
        /// <param name="logger">The logger used for logging purposes within the repository.</param>
        /// <remarks>
        /// The constructor sets up the database context options for PostgreSQL and invokes the base class constructor.
        /// It also ensures that the repository has access to the necessary SQL connection, HTTP context, and logging mechanism.
        /// </remarks>
        protected DbPostgeSqlConnectionEFRepositoryBase(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor, ILogger<DbPostgeSqlConnectionEFRepositoryBase<C, T>> logger)
            : base(CreateDbContextOptions(sqlConnectionFactory, ConnectionStringType.PostgreSqlConnection), sqlConnectionFactory, httpContextAccessor, logger)
        {
        }
    }
}