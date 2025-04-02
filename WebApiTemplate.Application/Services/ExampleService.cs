using AutoMapper;
using Microsoft.AspNetCore.Http;
using WebApiTemplate.Application.Dtos;
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
        public Task<ExampleDto> CreateAsync(object request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ExampleDto> GetByIdAsync(string id)
        {
            return _mapper.Map<ExampleDto>(await exampleRepository.GetExampleByIdAsync(id));
        }

        public Task<PaginatedResponse<ExampleDto>> SearchAsync(SearchRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExampleDto> UpdateAsync(object request)
        {
            throw new NotImplementedException();
        }
    }
}