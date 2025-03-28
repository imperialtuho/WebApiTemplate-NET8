using Microsoft.AspNetCore.Http;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Domain.Entities;
using WebApiTemplate.Infrastructure.Configurations;
using WebApiTemplate.Infrastructure.Database;

namespace WebApiTemplate.Infrastructure.Repositories.Providers.Example
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
                return new ExampleEntity() { Name = "This is example!" };
            }

            return await GetEntityByIdAsync(id);
        }
    }
}