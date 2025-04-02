using System.Data;
using WebApiTemplate.Domain.Enums;

namespace WebApiTemplate.Application.Configurations.Database
{
    /// <summary>
    /// Defines a factory for creating and managing SQL database connections.
    /// </summary>
    /// <remarks>
    /// This interface provides methods to retrieve and manage database connections. It supports the management of
    /// both SQL Server and PostgreSQL connections by allowing the configuration of connection string types
    /// and provides methods to retrieve or create database connections as needed.
    /// </remarks>
    public interface ISqlConnectionFactory
    {
        /// <summary>
        /// Retrieves an open database connection. If a connection is already open, it returns the existing one;
        /// otherwise, it creates and opens a new connection.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> instance that is open and ready for use.</returns>
        /// <remarks>
        /// This method ensures that the connection is open and ready for querying. It will either return an existing
        /// open connection or create and open a new one based on the connection string configuration.
        /// </remarks>
        IDbConnection GetOpenConnection();

        /// <summary>
        /// Creates a new database connection without opening it. This ensures a fresh connection instance each time.
        /// </summary>
        /// <returns>A new instance of <see cref="IDbConnection"/> that is not yet opened.</returns>
        /// <remarks>
        /// This method creates a new database connection instance based on the configured connection string type,
        /// but does not open the connection. This allows the caller to control when to open the connection.
        /// </remarks>
        IDbConnection GetNewConnection();

        /// <summary>
        /// Sets the type of connection string to use for establishing database connections.
        /// </summary>
        /// <param name="connectionStringType">The type of connection string to be used.</param>
        /// <remarks>
        /// This method allows for the selection of the type of connection string, such as SQL Server or PostgreSQL,
        /// which will be used for database connection creation. This enables flexibility in managing different types of databases.
        /// </remarks>
        void SetConnectionStringType(ConnectionStringType connectionStringType);

        /// <summary>
        /// Retrieves the current connection string along with the associated database type.
        /// </summary>
        /// <returns>A tuple containing the connection string (or null if not set) and the corresponding database type.</returns>
        /// <remarks>
        /// This method provides the current connection string and its associated database type,
        /// allowing the caller to access and manage the connection configuration details.
        /// </remarks>
        (string? connectionString, ConnectionStringType dbType) GetConnectionStringAndDbType();
    }
}