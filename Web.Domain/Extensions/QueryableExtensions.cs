using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Web.Domain.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> IncludeAllNavigations<T>(this IQueryable<T> query, DbContext dbContext) where T : class
        {
            IEntityType? entityType = dbContext.Model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");
            IEnumerable<INavigation> navigations = entityType.GetNavigations();

            foreach (INavigation navigation in navigations)
            {
                // Safely include navigation properties
                query = query.Include(navigation.Name);
            }

            return query;
        }
    }
}