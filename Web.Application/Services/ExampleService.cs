using AutoMapper;
using Mapster;
using Microsoft.AspNetCore.Http;
using Web.Application.Dtos.Base;
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
        public Task<BaseDto> CreateAsync(MediaAddRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseDto> GetByIdAsync(string id)
        {
            return (await exampleRepository.GetExampleByIdAsync(id)).Adapt<BaseDto>();
        }

        public Task<PaginatedResponse<BaseDto>> SearchAsync(SearchRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BaseDto> UpdateAsync(MediaUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}