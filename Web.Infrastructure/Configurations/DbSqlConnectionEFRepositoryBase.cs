using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Web.Application.Configurations.Database;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Infrastructure.Repositories.Providers;

namespace Web.Infrastructure.Configurations
{
    public abstract class DbSqlConnectionEFRepositoryBase<C, T> : EntityFrameworkGenericRepository<C, T>
        where T : BaseEntity<string>
        where C : DbContext, new()
    {
        protected DbSqlConnectionEFRepositoryBase(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor)
            : base(CreateDbContextOptions(sqlConnectionFactory, ConnectionStringType.SqlServerConnection), sqlConnectionFactory, httpContextAccessor)
        {
        }
    }
}