using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.Domain.Enums;
using WebApiTemplate.Infrastructure.Repositories.Providers;

namespace WebApiTemplate.Infrastructure.Configurations
{
    /// <summary>
    /// A base repository that provides Entity Framework-based database operations
    /// for SQL Server connections.
    /// </summary>
    /// <typeparam name="C">The database context type, inheriting from DbContext.</typeparam>
    /// <typeparam name="T">The entity type, inheriting from BaseEntity<string>.</typeparam>
    public abstract class DbSqlConnectionEFRepositoryBase<C, T> : EntityFrameworkGenericRepository<C, T>
        where T : BaseEntity<string>
        where C : DbContext, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbSqlConnectionEFRepositoryBase{C, T}"/> class.
        /// </summary>
        /// <param name="sqlConnectionFactory">The SQL connection factory for database connections.</param>
        /// <param name="httpContextAccessor">Provides access to the HTTP context.</param>
        protected DbSqlConnectionEFRepositoryBase(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor)
            : base(CreateDbContextOptions(sqlConnectionFactory, ConnectionStringType.SqlServerConnection), sqlConnectionFactory, httpContextAccessor)
        {
        }
    }
}