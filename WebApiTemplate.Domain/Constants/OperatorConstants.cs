namespace WebApiTemplate.Domain.Constants
{
    /// <summary>
    /// Defines constant values for supported filter operations used in query building.
    /// </summary>
    /// <remarks>
    /// These constants represent string identifiers for various comparison operators.
    /// They are typically used in conjunction with the <see cref="Common.FilterCriteria.Operator"/> property
    /// to dynamically apply filters in query expressions.
    /// </remarks>
    public static class OperatorConstants
    {
        /// <summary>
        /// Represents an equality comparison ("eq").
        /// </summary>
        public const string Equal = "eq";

        /// <summary>
        /// Represents an inequality comparison ("ne").
        /// </summary>
        public const string NotEqual = "ne";

        /// <summary>
        /// Represents a "less than" comparison ("lt").
        /// </summary>
        public const string LessThan = "lt";

        /// <summary>
        /// Represents a "less than or equal to" comparison ("lte").
        /// </summary>
        public const string LessThanEqual = "lte";

        /// <summary>
        /// Represents a "greater than" comparison ("gt").
        /// </summary>
        public const string GreaterThan = "gt";

        /// <summary>
        /// Represents a "greater than or equal to" comparison ("gte").
        /// </summary>
        public const string GreaterThanEqual = "gte";

        /// <summary>
        /// Represents a "contains" operation, used for partial string matching ("ct").
        /// </summary>
        public const string Contains = "ct";

        /// <summary>
        /// Represents a range comparison between two values ("btw").
        /// </summary>
        public const string Between = "btw";

        /// <summary>
        /// Represents a "starts with" string operation ("stw").
        /// </summary>
        public const string StartsWith = "stw";

        /// <summary>
        /// Represents an "ends with" string operation ("enw").
        /// </summary>
        public const string EndsWith = "enw";
    }
}