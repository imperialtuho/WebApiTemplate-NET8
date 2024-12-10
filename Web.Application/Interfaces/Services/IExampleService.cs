using Web.Application.Dtos.Base;
using Web.Application.Dtos.Media;
using Web.Domain.Common;

namespace Web.Application.Interfaces.Services
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