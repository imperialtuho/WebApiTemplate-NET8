using Web.Domain.Entities;

namespace Web.Application.Interfaces.Repositories
{
    public interface IExampleRepository : IEntityFrameworkGenericRepository<Example>
    {
        Task<IList<Example>> GetByIdsAsync(IList<string> ids);
    }
}