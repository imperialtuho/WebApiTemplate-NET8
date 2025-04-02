using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApiTemplate.Domain.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IQueryable{T}"/> to simplify entity querying and include navigation properties dynamically.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Includes all navigation properties for a given entity type in a query.
        /// This method dynamically includes all navigation properties (such as relationships between entities) in the query.
        /// It helps to load related entities automatically without explicitly specifying each navigation property.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="query">The query to extend with navigation properties.</param>
        /// <param name="dbContext">The database context containing the entity model.</param>
        /// <returns>An <see cref="IQueryable{T}"/> with all navigation properties included, allowing for eager loading.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the entity type is not found in the model.</exception>
        /// <remarks>
        /// This extension method simplifies the inclusion of navigation properties, especially when there are many relationships between entities.
        /// It automatically includes all related entities in the query, reducing the need for manual inclusion.
        /// </remarks>
        public static IQueryable<T> IncludeAllNavigations<T>(this IQueryable<T> query, DbContext dbContext) where T : class
        {
            // Retrieve the entity type from the model metadata using the DbContext
            IEntityType? entityType = dbContext.Model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");

            // Get all navigation properties (relationships between entities)
            IEnumerable<INavigation> navigations = entityType.GetNavigations();

            // Iterate over the navigation properties and include them in the query
            foreach (INavigation navigation in navigations)
            {
                // Include each navigation property by name in the query
                query = query.Include(navigation.Name);
            }

            // Return the modified query with all navigation properties included
            return query;
        }
    }
}