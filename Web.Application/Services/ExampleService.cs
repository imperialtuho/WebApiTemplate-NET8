using AutoMapper;
using Microsoft.AspNetCore.Http;
using Web.Application.Dtos.Media;
using Web.Application.Interfaces.ExternalProviders;
using Web.Application.Interfaces.Repositories;
using Web.Application.Interfaces.Services;
using Web.Domain.Common;

namespace Web.Application.Services
{
    public class ExampleService(IExampleRepository exampleRepository,
        IIdentityApi identityApi,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper) : BaseService(httpContextAccessor, mapper), IExampleService
    {
        public Task<MediaDto> CreateAsync(MediaAddRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<MediaDto> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResponse<MediaDto>> SearchAsync(SearchRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MediaDto> UpdateAsync(MediaUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}