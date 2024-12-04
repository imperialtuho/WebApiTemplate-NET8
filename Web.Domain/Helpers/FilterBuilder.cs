using System.Linq.Expressions;
using System.Reflection;

namespace Web.Domain.Helpers
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

                    if (propertyInfo != null)
                    {
                        var parameter = Expression.Parameter(typeof(T), "e");
                        var property = Expression.Property(parameter, propertyInfo);
                        var value = Expression.Constant(Convert.ChangeType(filter.Value, propertyInfo.PropertyType));
                        Expression? comparison = null;

                        if (propertyInfo.PropertyType == typeof(string))
                        {
                            MethodInfo? containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
                            comparison = Expression.Call(property, containsMethod!, value);
                        }
                        else if (propertyInfo.PropertyType.IsValueType)
                        {
                            comparison = Expression.Equal(property, value);
                        }

                        if (comparison != null)
                        {
                            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                            query = query.Where(lambda);
                        }
                    }
                }

                return query;
            };
        }
    }
}