using Mapster;
using Web.Application.Configurations.Database;
using Web.Application.Dtos;
using Web.Application.Interfaces.Repositories;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Infrastructure.Configurations;
using Web.Infrastructure.Database;

namespace Web.Infrastructure.Repositories.Providers.Blogs
{
    public class BlogRepository : DbSqlConnectionEFRepositoryBase<ApplicationDbContext, Blog>, IBlogRepository
    {
        public BlogRepository(ISqlConnectionFactory sqlConnectionFactory) : base(sqlConnectionFactory)
        {
            sqlConnectionFactory.SetConnectionStringType(ConnectionStringType.SqlServerConnection);
        }

        public async Task<Blog> CreateAsync(BlogDto blog)
        {
            if (blog == null)
            {
                throw new InvalidOperationException($"{nameof(blog)} cannot be null.");
            }

            var blogToAdd = blog.Adapt<Blog>();

            await AddAndSaveChangesAsync(blogToAdd);

            return blogToAdd;
        }

        public async Task<Blog> GetByIdAsync(string id)
        {
            return await GetEntityByIdAsync(id);
        }
    }
}