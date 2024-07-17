using Web.Application.Dtos;

namespace Web.Application.Interfaces.Services
{
    public interface IBlogService
    {
        Task<BlogDto> GetByIdAsync(string id);
    }
}