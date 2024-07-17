using Mapster;
using Web.Application.Dtos;
using Web.Application.Interfaces.Repositories;
using Web.Application.Interfaces.Services;

namespace Web.Application.Services
{
    public class BlogService(IBlogRepository blogRepository) : IBlogService
    {
        public async Task<BlogDto> GetByIdAsync(string id)
        {
            return (await blogRepository.GetByIdAsync(id)).Adapt<BlogDto>();
        }
    }
}