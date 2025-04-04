using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using WebApiTemplate.Domain.Common;
using WebApiTemplate.Domain.Constants;

namespace WebApiTemplate.Domain.Helpers
{
    /// <summary>
    /// Provides dynamic filtering capabilities for an <see cref="IQueryable{T}"/> collection
    /// based on a list of filter criteria.
    /// </summary>
    /// <typeparam name="T">The type of entity to filter.</typeparam>
    /// <remarks>
    /// This class builds a dynamic filtering expression for LINQ queries based on a set of filter criteria.
    /// The filtering logic utilizes <see cref="Expression"/> to dynamically build
    /// predicate expressions for filtering in LINQ queries.
    /// </remarks>
    public class FilterBuilder<T>
    {
        private readonly List<FilterCriteria> _filters;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterBuilder{T}"/> class with a set of filter criteria.
        /// </summary>
        /// <param name="filters">A list of <see cref="FilterCriteria"/> defining the filtering conditions.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="filters"/> list is null.</exception>
        public FilterBuilder(List<FilterCriteria> filters)
        {
            _filters = filters ?? throw new ArgumentNullException(nameof(filters), message: "Filters cannot be null.");
        }

        /// <summary>
        /// Constructs a dynamic filtering function based on a set of filter criteria that can be applied to an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <returns>
        /// A function that accepts an <see cref="IQueryable{T}"/> and returns a filtered <see cref="IQueryable{T}"/> based on the defined criteria.
        /// </returns>
        /// <remarks>
        /// This method iterates through a list of <see cref="FilterCriteria"/> objects, building expression trees dynamically
        /// for supported operators such as equality, inequality, comparison, string operations (e.g., Contains, StartsWith, EndsWith),
        /// and range filtering using Between. Each valid filter is translated into a LINQ expression and applied to the source query.
        ///
        /// Type conversions are handled automatically, including nullable value types, enums, and common types like DateTime or Guid.
        /// If a property or value is invalid or unsupported, that filter is skipped.
        /// </remarks>
        public Func<IQueryable<T>, IQueryable<T>> Build()
        {
            return query =>
            {
                foreach (FilterCriteria filter in _filters)
                {
                    PropertyInfo? propertyInfo = typeof(T).GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null) continue;

                    Type targetType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    ParameterExpression parameter = Expression.Parameter(typeof(T), "e");
                    MemberExpression property = Expression.Property(parameter, propertyInfo);
                    Expression? comparison = null;

                    object? convertedValue = ConvertValue(filter.Value, targetType);
                    object? convertedValue2 = filter.Value2 != null ? ConvertValue(filter.Value2, targetType) : null;

                    if (convertedValue == null) continue;

                    switch (filter.Operator)
                    {
                        case OperatorConstants.Equal:
                            if (property.Type == typeof(string) && !filter.CaseSensitive)
                            {
                                MethodInfo? toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
                                MethodCallExpression? left = Expression.Call(property, toLower!);
                                ConstantExpression? right = Expression.Constant(convertedValue.ToString()?.ToLower(), typeof(string));
                                comparison = Expression.Equal(left, right);
                            }
                            else
                            {
                                comparison = Expression.Equal(property, Expression.Constant(convertedValue, property.Type));
                            }
                            break;

                        case OperatorConstants.NotEqual:
                            if (property.Type == typeof(string) && !filter.CaseSensitive)
                            {
                                MethodInfo? toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
                                MethodCallExpression? left = Expression.Call(property, toLower!);
                                ConstantExpression? right = Expression.Constant(convertedValue.ToString()?.ToLower(), typeof(string));
                                comparison = Expression.NotEqual(left, right);
                            }
                            else
                            {
                                comparison = Expression.NotEqual(property, Expression.Constant(convertedValue, property.Type));
                            }
                            break;

                        case OperatorConstants.GreaterThan:
                            comparison = Expression.GreaterThan(property, Expression.Constant(convertedValue, property.Type));
                            break;

                        case OperatorConstants.GreaterThanEqual:
                            comparison = Expression.GreaterThanOrEqual(property, Expression.Constant(convertedValue, property.Type));
                            break;

                        case OperatorConstants.LessThan:
                            comparison = Expression.LessThan(property, Expression.Constant(convertedValue, property.Type));
                            break;

                        case OperatorConstants.LessThanEqual:
                            comparison = Expression.LessThanOrEqual(property, Expression.Constant(convertedValue, property.Type));
                            break;

                        case OperatorConstants.Contains:
                            if (propertyInfo.PropertyType == typeof(string))
                            {
                                MethodInfo? containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
                                if (!filter.CaseSensitive)
                                {
                                    MethodInfo? toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
                                    MethodCallExpression? propertyToLower = Expression.Call(property, toLower!);
                                    ConstantExpression? valueToLower = Expression.Constant(convertedValue.ToString()?.ToLower(), typeof(string));
                                    BinaryExpression? notNull = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));
                                    comparison = Expression.AndAlso(notNull, Expression.Call(propertyToLower, containsMethod!, valueToLower));
                                }
                                else
                                {
                                    comparison = Expression.Call(property, containsMethod!, Expression.Constant(convertedValue, typeof(string)));
                                }
                            }
                            break;

                        case OperatorConstants.StartsWith:
                            if (propertyInfo.PropertyType == typeof(string))
                            {
                                MethodInfo? startsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)]);
                                if (!filter.CaseSensitive)
                                {
                                    MethodInfo? toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
                                    MethodCallExpression propertyToLower = Expression.Call(property, toLower!);
                                    ConstantExpression valueToLower = Expression.Constant(convertedValue.ToString()?.ToLower(), typeof(string));
                                    BinaryExpression notNull = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));
                                    comparison = Expression.AndAlso(notNull, Expression.Call(propertyToLower, startsWithMethod!, valueToLower));
                                }
                                else
                                {
                                    comparison = Expression.Call(property, startsWithMethod!, Expression.Constant(convertedValue, typeof(string)));
                                }
                            }
                            break;

                        case OperatorConstants.EndsWith:
                            if (propertyInfo.PropertyType == typeof(string))
                            {
                                MethodInfo? endsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)]);
                                if (!filter.CaseSensitive)
                                {
                                    MethodInfo? toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
                                    MethodCallExpression propertyToLower = Expression.Call(property, toLower!);
                                    ConstantExpression valueToLower = Expression.Constant(convertedValue.ToString()?.ToLower(), typeof(string));
                                    BinaryExpression notNull = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));
                                    comparison = Expression.AndAlso(notNull, Expression.Call(propertyToLower, endsWithMethod!, valueToLower));
                                }
                                else
                                {
                                    comparison = Expression.Call(property, endsWithMethod!, Expression.Constant(convertedValue, typeof(string)));
                                }
                            }
                            break;

                        case OperatorConstants.Between:
                            if (convertedValue2 != null)
                            {
                                ConstantExpression value1 = Expression.Constant(convertedValue, property.Type);
                                ConstantExpression value2 = Expression.Constant(convertedValue2, property.Type);
                                Expression lowerBound = Expression.GreaterThanOrEqual(property, value1);
                                Expression upperBound = Expression.LessThanOrEqual(property, value2);
                                comparison = Expression.AndAlso(lowerBound, upperBound);
                            }
                            break;
                    }

                    if (comparison != null)
                    {
                        Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                        query = query.Where(lambda);
                    }
                }

                return query;
            };
        }

        /// <summary>
        /// Converts a string value to a strongly-typed object based on the target property type.
        /// </summary>
        /// <param name="value">The string representation of the value.</param>
        /// <param name="targetType">The expected type to convert the value into.</param>
        /// <returns>
        /// The converted value as an object, or <c>null</c> if conversion fails.
        /// </returns>
        /// <remarks>
        /// This method ensures proper type conversion for various data types, including:
        /// <list type="bullet">
        /// <item><description><see cref="Guid"/></description></item>
        /// <item><description><see cref="DateTime"/> (parsed with <see cref="CultureInfo.InvariantCulture"/>)</description></item>
        /// <item><description><see cref="DateTimeOffset"/> (parsed with <see cref="CultureInfo.InvariantCulture"/>)</description></item>
        /// <item><description><see cref="TimeOnly"/> (parsed with <see cref="CultureInfo.InvariantCulture"/>)</description></item>
        /// <item><description><see cref="DateOnly"/> (parsed with <see cref="CultureInfo.InvariantCulture"/>)</description></item>
        /// <item><description>Any <see langword="enum"/> type</description></item>
        /// <item><description>Numeric and boolean values</description></item>
        /// </list>
        ///
        /// If the conversion fails, the method returns <c>null</c> to indicate an invalid input.
        /// </remarks>
        private static object? ConvertValue(string value, Type targetType)
        {
            try
            {
                if (targetType == typeof(Guid))
                    return Guid.Parse(value);

                if (targetType == typeof(DateTime))
                    return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                if (targetType == typeof(DateTimeOffset))
                    return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                if (targetType == typeof(TimeOnly))
                    return TimeOnly.Parse(value, CultureInfo.InvariantCulture);

                if (targetType == typeof(DateOnly))
                    return DateOnly.Parse(value, CultureInfo.InvariantCulture);

                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, true);

                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}