using WebApiTemplate.Domain.Entities;

namespace WebApiTemplate.Application.Interfaces.Repositories
{
    public interface IExampleRepository : IEntityFrameworkGenericRepository<ExampleEntity>
    {
        Task<ExampleEntity> GetExampleByIdAsync(string id);
    }
}