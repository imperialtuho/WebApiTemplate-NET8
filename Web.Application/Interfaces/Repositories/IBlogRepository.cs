using Web.Application.Dtos;
using Web.Domain.Entities;

namespace Web.Application.Interfaces.Repositories
{
    public interface IBlogRepository
    {
        Task<Blog> GetByIdAsync(string id);

        Task<Blog> CreateAsync(BlogDto blog);
    }
}