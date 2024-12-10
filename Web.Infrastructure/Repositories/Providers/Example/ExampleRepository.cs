using Microsoft.AspNetCore.Http;
using Web.Application.Configurations.Database;
using Web.Application.Interfaces.Repositories;
using Web.Domain.Entities;
using Web.Infrastructure.Configurations;
using Web.Infrastructure.Database;

namespace Web.Infrastructure.Repositories.Providers.Blogs
{
    public class ExampleRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Example>, IExampleRepository
    {
        public ExampleRepository(ISqlConnectionFactory sqlConnectionFactory, IHttpContextAccessor httpContextAccessor) : base(sqlConnectionFactory, httpContextAccessor)
        {
        }

        public async Task<Example> GetExampleByIdAsync(string id)
        {
            bool isMocking = string.IsNullOrWhiteSpace(id);

            if (isMocking)
            {
                return new Example() { };
            }

            return await GetEntityByIdAsync(id);
        }
    }
}