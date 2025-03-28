using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.Domain.Enums;
using WebApiTemplate.Infrastructure.Repositories.Providers;

namespace WebApiTemplate.Infrastructure.Configurations
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