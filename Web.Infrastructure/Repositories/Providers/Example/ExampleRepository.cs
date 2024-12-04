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

        public Task<IList<Example>> GetByIdsAsync(IList<string> ids)
        {
            throw new NotImplementedException();
        }
    }
}