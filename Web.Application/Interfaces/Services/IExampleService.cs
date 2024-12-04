using Web.Application.Dtos.Media;
using Web.Domain.Common;

namespace Web.Application.Interfaces.Services
{
    public interface IExampleService
    {
        Task<PaginatedResponse<MediaDto>> SearchAsync(SearchRequest request);

        Task<MediaDto> GetByIdAsync(string id);

        Task<MediaDto> CreateAsync(MediaAddRequest request);

        Task<MediaDto> UpdateAsync(MediaUpdateRequest request);

        Task<bool> DeleteAsync(string id);
    }
}