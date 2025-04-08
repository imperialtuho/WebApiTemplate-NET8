using WebApiTemplate.Domain.Constants;

namespace WebApiTemplate.Domain.Common
{
    /// <summary>
    /// Represents a filtering condition used in dynamic LINQ queries.
    /// </summary>
    /// <remarks>
    /// This class is typically used by <see cref="Helpers.FilterBuildingHelper{T}"/> to construct expressions based on dynamic filtering logic.
    /// The <see cref="Field"/> represents the property to filter on, while the <see cref="Operator"/> determines the type of comparison.
    /// The <see cref="Value"/> and <see cref="Value2"/> hold the values used for comparison, with <see cref="Value2"/> being optional for "between" filters.
    /// The <see cref="CaseSensitive"/> flag determines whether the filter should apply case-sensitive comparisons for string values.
    /// </remarks>
    public class FilterCriteria
    {
        /// <summary>
        /// Gets or sets the name of the property to filter on.
        /// </summary>
        /// <value>The name of the property to filter on (case-insensitive).</value>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the comparison operator to use for the filter.
        /// </summary>
        /// <value>The operator to apply (e.g., "eq", "ne", "lt", "gt", etc.). Defaults to "eq" (Equal).</value>
        public string Operator { get; set; } = OperatorConstants.Equal;  // Default to Equals

        /// <summary>
        /// Gets or sets a value indicating whether the filter comparison should be case-sensitive.
        /// </summary>
        /// <value>True if case-sensitive comparisons should be applied; otherwise, false.</value>
        public bool CaseSensitive { get; set; } = false;

        /// <summary>
        /// Gets or sets the primary comparison value for the filter.
        /// </summary>
        /// <value>The value to compare the <see cref="Field"/> to.</value>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the secondary value for range comparisons (used for "between" filters).
        /// </summary>
        /// <value>The optional second comparison value (used with the "between" operator).</value>
        public string? Value2 { get; set; } // Used for "between" operator
    }
}