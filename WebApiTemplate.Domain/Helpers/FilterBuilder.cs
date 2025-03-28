using System.Linq.Expressions;
using System.Reflection;

namespace WebApiTemplate.Domain.Helpers
{
    public class FilterBuilder<T>
    {
        private readonly Dictionary<string, string> _filters;

        public FilterBuilder(Dictionary<string, string> filters)
        {
            _filters = filters;
        }

        public Func<IQueryable<T>, IQueryable<T>> Build()
        {
            return query =>
            {
                foreach (KeyValuePair<string, string> filter in _filters)
                {
                    PropertyInfo? propertyInfo = typeof(T).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        // Skip if property does not exist
                        continue;
                    }

                    var parameter = Expression.Parameter(typeof(T), "e");
                    var property = Expression.Property(parameter, propertyInfo);

                    // Handle Nullable<T> by getting its underlying type
                    Type targetType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                    object? convertedValue;
                    try
                    {
                        convertedValue = Convert.ChangeType(filter.Value, targetType);
                    }
                    catch
                    {
                        // Skip filter if conversion fails
                        continue;
                    }

                    var value = Expression.Constant(convertedValue);
                    Expression? comparison = null;

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        MethodInfo? containsMethod = typeof(string).GetMethod("Contains", [typeof(string)], null);
                        MethodInfo? toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

                        if (containsMethod != null && toLowerMethod != null)
                        {
                            // Convert both property and value to lowercase for case-insensitive search
                            var lowerProperty = Expression.Call(property, toLowerMethod);
                            var lowerValue = Expression.Call(value, toLowerMethod);
                            comparison = Expression.Call(lowerProperty, containsMethod, lowerValue);
                        }
                    }
                    else if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
                    {
                        comparison = Expression.Equal(property, value);
                    }

                    if (comparison != null)
                    {
                        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                        query = query.Where(lambda);
                    }
                }

                return query;
            };
        }
    }
}