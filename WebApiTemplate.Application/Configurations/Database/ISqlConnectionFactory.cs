using System.Data;
using WebApiTemplate.Domain.Enums;

namespace WebApiTemplate.Application.Configurations.Database
{
    /// <summary>
    /// Defines a factory for creating and managing SQL database connections.
    /// </summary>
    public interface ISqlConnectionFactory
    {
        /// <summary>
        /// Retrieves an open database connection. If a connection is already open, it returns the existing one;
        /// otherwise, it creates and opens a new connection.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> instance that is open and ready for use.</returns>
        IDbConnection GetOpenConnection();

        /// <summary>
        /// Creates a new database connection without opening it. This ensures a fresh connection instance each time.
        /// </summary>
        /// <returns>A new instance of <see cref="IDbConnection"/> that is not yet opened.</returns>
        IDbConnection GetNewConnection();

        /// <summary>
        /// Sets the type of connection string to use for establishing database connections.
        /// </summary>
        /// <param name="connectionStringType">The type of connection string to be used.</param>
        void SetConnectionStringType(ConnectionStringType connectionStringType);

        /// <summary>
        /// Retrieves the current connection string along with the associated database type.
        /// </summary>
        /// <returns>A tuple containing the connection string (or null if not set) and the corresponding database type.</returns>
        (string? connectionString, ConnectionStringType dbType) GetConnectionStringAndDbType();
    }
}