namespace WebApiTemplate.Domain.Enums
{
    /// <summary>
    /// Enum representing the different types of connection strings used in the application.
    /// </summary>
    public enum ConnectionStringType
    {
        /// <summary>
        /// No specific connection string type.
        /// </summary>
        None = 0,

        /// <summary>
        /// The default connection string type.
        /// </summary>
        DefaultConnection = 1,

        /// <summary>
        /// Represents a SQL Server connection string.
        /// </summary>
        SqlServerConnection = 2,

        /// <summary>
        /// Represents a PostgreSQL connection string.
        /// </summary>
        PostgreSqlConnection = 3
    }
}