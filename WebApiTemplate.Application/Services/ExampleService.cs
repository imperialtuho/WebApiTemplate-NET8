using AutoMapper;
using Microsoft.AspNetCore.Http;
using WebApiTemplate.Application.Dtos;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Application.Interfaces.Services;
using WebApiTemplate.Domain.Common;
using WebApiTemplate.Domain.Exceptions;
using WebApiTemplate.Domain.Helpers;

namespace WebApiTemplate.Application.Services
{
    public class ExampleService(IExampleRepository exampleRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : BaseService(httpContextAccessor, mapper), IExampleService
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
            _ = LoginSession ?? throw new ForbiddenException();
            // Step 1: Create a list of products
            IQueryable<ExampleDto>? products = ProductInitialize().AsQueryable();

            // Step 2: Use the FilterBuilder to apply the filters
            FilterBuilder<ExampleDto>? filterBuilder = new(request.Filters ?? []);
            Func<IQueryable<ExampleDto>, IQueryable<ExampleDto>>? filterExpression = filterBuilder.Build();
            IQueryable<ExampleDto> filteredProducts = filterExpression(products);

            // Step 3: Apply pagination
            PaginatedResponse<ExampleDto>? result = PaginatedResponse<ExampleDto>.Create(filteredProducts, request.PageNumber, request.PageSize);

            return Task.FromResult(result);
        }

        public Task<ExampleDto> UpdateAsync(object request)
        {
            throw new NotImplementedException();
        }

        private static List<ExampleDto> ProductInitialize()
        {
            return new List<ExampleDto>
            {
                new () { Id = "1", Name = "Laptop",     Price = 800,    Category = "Electronics",   CreatedDate = new DateTime(2025, 1, 1,0,0,0, DateTimeKind.Utc) },
                new () { Id = "2", Name = "Phone",      Price = 800,    Category = "Electronics",   CreatedDate = new DateTime(2025, 2, 2,0,0,0, DateTimeKind.Utc) },
                new () { Id = "3", Name = "Shirt",      Price = 25,     Category = "Clothing",      CreatedDate = new DateTime(2025, 3, 3,0,0,0, DateTimeKind.Utc) },
                new () { Id = "4", Name = "Headphones", Price = 200,    Category = "Electronics",   CreatedDate = new DateTime(2025, 4, 4,0,0,0, DateTimeKind.Utc) },
                new () { Id = "5", Name = "Pants",      Price = 30,     Category = "Clothing",      CreatedDate = new DateTime(2025, 5, 5,0,0,0, DateTimeKind.Utc) },
                new () { Id = "6", Name = "Laptop",     Price = 1000,   Category = "Electronics",   CreatedDate = new DateTime(2025, 5, 5,0,0,0, DateTimeKind.Utc) },
                new () { Id = "7", Name = "Rooftop",    Price = 1000,   Category = "Electronics",   CreatedDate = new DateTime(2025, 5, 5,0,0,0, DateTimeKind.Utc) }
            };
        }
    }
}