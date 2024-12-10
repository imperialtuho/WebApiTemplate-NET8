using Web.Domain.Entities;

namespace Web.Application.Interfaces.Repositories
{
    public interface IExampleRepository : IEntityFrameworkGenericRepository<Example>
    {
        Task<Example> GetExampleByIdAsync(string id);
    }
}