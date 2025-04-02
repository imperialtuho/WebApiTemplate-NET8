using WebApiTemplate.Application.Dtos;
using WebApiTemplate.Domain.Common;

namespace WebApiTemplate.Application.Interfaces.Services
{
    public interface IExampleService
    {
        Task<PaginatedResponse<ExampleDto>> SearchAsync(SearchRequest request);

        Task<ExampleDto> GetByIdAsync(string id);

        Task<ExampleDto> CreateAsync(object request);

        Task<ExampleDto> UpdateAsync(object request);

        Task<bool> DeleteAsync(string id);
    }
}