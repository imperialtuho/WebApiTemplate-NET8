using AutoMapper;
using Mapster;
using Microsoft.AspNetCore.Http;
using WebApiTemplate.Application.Dtos.Base;
using WebApiTemplate.Application.Dtos.Media;
using WebApiTemplate.Application.Interfaces.ExternalProviders;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Application.Interfaces.Services;
using WebApiTemplate.Domain.Common;

namespace WebApiTemplate.Application.Services
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