using Microsoft.AspNetCore.Http;
using Web.Application.Configurations.Database;
using Web.Application.Interfaces.Repositories;
using Web.Domain.Entities;
using Web.Infrastructure.Configurations;
using Web.Infrastructure.Database;

namespace Web.Infrastructure.Repositories.Providers.Example
{
    public class ExampleRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, ExampleEntity>, IExampleRepository
    {
        public ExampleRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
        }

        public async Task<ExampleEntity> GetExampleByIdAsync(string id)
        {
            bool isMocking = string.IsNullOrWhiteSpace(id);

            if (isMocking)
            {
                return new ExampleEntity() { };
            }

            return await GetEntityByIdAsync(id);
        }
    }
}