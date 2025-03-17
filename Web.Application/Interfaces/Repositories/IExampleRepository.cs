using Web.Domain.Entities;

namespace Web.Application.Interfaces.Repositories
{
    public interface IExampleRepository : IEntityFrameworkGenericRepository<ExampleEntity>
    {
        Task<ExampleEntity> GetExampleByIdAsync(string id);
    }
}