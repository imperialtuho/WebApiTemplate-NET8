using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApiTemplate.Domain.Extensions
{
    /// <summary>
    /// IQueryable extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Includes all navigation properties for a given entity type in a query.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="query">The query to extend with navigation properties.</param>
        /// <param name="dbContext">The database context containing the entity model.</param>
        /// <returns>An <see cref="IQueryable{T}"/> with all navigation properties included.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the entity type is not found in the model.</exception>
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