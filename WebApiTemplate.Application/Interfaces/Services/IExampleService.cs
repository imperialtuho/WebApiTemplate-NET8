using WebApiTemplate.Application.Dtos.Base;
using WebApiTemplate.Application.Dtos.Media;
using WebApiTemplate.Domain.Common;

namespace WebApiTemplate.Application.Interfaces.Services
{
    public interface IExampleService
    {
        Task<PaginatedResponse<BaseDto>> SearchAsync(SearchRequest request);

        Task<BaseDto> GetByIdAsync(string id);

        Task<BaseDto> CreateAsync(MediaAddRequest request);

        Task<BaseDto> UpdateAsync(MediaUpdateRequest request);

        Task<bool> DeleteAsync(string id);
    }
}