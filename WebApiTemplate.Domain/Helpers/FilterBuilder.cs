using System.Linq.Expressions;
using System.Reflection;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides a dynamic filtering mechanism for <see cref="IQueryable{T}"/>.
    /// This class constructs query filters based on a dictionary of property names and values.
    /// </summary>
    /// <typeparam name="T">The type of entity to filter.</typeparam>
    public class FilterBuilder<T>
    {
        private readonly Dictionary<string, string> _filters;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterBuilder{T}"/> class.
        /// </summary>
        /// <param name="filters">
        /// A dictionary where the key is the property name of <typeparamref name="T"/> and
        /// the value is the filter criteria to be applied. Each key-value pair corresponds to
        /// a filter to be applied on the corresponding property of <typeparamref name="T"/>.
        /// </param>
        /// <remarks>
        /// This constructor initializes the filter builder with a set of filters that can be
        /// applied dynamically to an <see cref="IQueryable{T}"/>. The filters are expected to
        /// correspond to properties of the entity type <typeparamref name="T"/>.
        /// </remarks>
        public FilterBuilder(Dictionary<string, string> filters)
        {
            _filters = filters ?? throw new ArgumentNullException(nameof(filters), "Filters cannot be null.");
        }

        /// <summary>
        /// Builds a filter expression that can be applied to an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <returns>
        /// A function that takes an <see cref="IQueryable{T}"/> and returns a filtered <see cref="IQueryable{T}"/>.
        /// The function applies all filters based on the provided dictionary of filters.
        /// </returns>
        /// <remarks>
        /// This method constructs a series of dynamic expressions based on the filters provided,
        /// allowing for runtime construction of a query that can filter data according to the
        /// specified property values. It supports filtering for string properties using a case-insensitive
        /// "Contains" comparison and equality checks for value types.
        /// </remarks>
        public Func<IQueryable<T>, IQueryable<T>> Build()
        {
            return query =>
            {
                foreach (KeyValuePair<string, string> filter in _filters)
                {
                    // Get the property info of the target type
                    PropertyInfo? propertyInfo = typeof(T).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        // Skip if property does not exist
                        continue;
                    }

                    ParameterExpression? parameter = Expression.Parameter(typeof(T), "e");
                    MemberExpression? property = Expression.Property(parameter, propertyInfo);

                    // Handle Nullable<T> by getting its underlying type
                    Type targetType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                    object? convertedValue;

                    try
                    {
                        // Convert filter value to the appropriate type
                        convertedValue = Convert.ChangeType(filter.Value, targetType);
                    }
                    catch
                    {
                        // Skip filter if conversion fails
                        continue;
                    }

                    ConstantExpression? value = Expression.Constant(convertedValue);
                    Expression? comparison = null;

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        // Perform case-insensitive "Contains" filtering on strings
                        MethodInfo? containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        MethodInfo? toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

                        if (containsMethod != null && toLowerMethod != null)
                        {
                            // Convert both property and value to lowercase for case-insensitive search
                            MethodCallExpression? lowerProperty = Expression.Call(property, toLowerMethod);
                            MethodCallExpression? lowerValue = Expression.Call(value, toLowerMethod);
                            comparison = Expression.Call(lowerProperty, containsMethod, lowerValue);
                        }
                    }
                    else if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
                    {
                        // Perform equality comparison for value types and strings
                        comparison = Expression.Equal(property, value);
                    }

                    if (comparison != null)
                    {
                        Expression<Func<T, bool>>? lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                        query = query.Where(lambda);
                    }
                }

                return query;
            };
        }
    }
}