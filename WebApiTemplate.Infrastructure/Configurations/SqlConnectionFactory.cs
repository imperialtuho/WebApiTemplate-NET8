using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Domain.Enums;

namespace WebApiTemplate.Infrastructure.Configurations
{
    /// <summary>
    /// Factory for managing database connections, supporting any type of databases.
    /// </summary>
    /// <remarks>
    /// This class provides methods for obtaining database connections to SQL Server or PostgreSQL based on the connection string configuration.
    /// It ensures that only one open connection exists at a time and properly manages the lifecycle of the connection.
    /// It also supports switching between different database connection types.
    /// </remarks>
    public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IDbConnection? _connection;
        private bool _disposed = false;
        private ConnectionStringType _connectionStringType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration settings used to retrieve connection strings.</param>
        /// <remarks>
        /// The constructor takes the application's configuration to retrieve the appropriate connection string
        /// for either SQL Server or PostgreSQL or any databases depending on the `ConnectionStringType` set.
        /// </remarks>
        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gets an open database connection. If a connection is already open, it returns that.
        /// </summary>
        /// <returns>An open <see cref="IDbConnection"/> instance.</returns>
        /// <remarks>
        /// If no connection is open or the current connection is closed, this method creates a new connection
        /// based on the selected <see cref="ConnectionStringType"/> and opens it.
        /// </remarks>
        public IDbConnection GetOpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = _connectionStringType switch
                {
                    ConnectionStringType.PostgreSqlConnection => new NpgsqlConnection(_configuration.GetConnectionString(nameof(ConnectionStringType.PostgreSqlConnection))),
                    ConnectionStringType.SqlServerConnection => new SqlConnection(_configuration.GetConnectionString(nameof(ConnectionStringType.SqlServerConnection))),
                    ConnectionStringType.DefaultConnection => new SqlConnection(_configuration.GetConnectionString(nameof(ConnectionStringType.DefaultConnection))),
                    _ => new SqlConnection(_configuration.GetConnectionString(nameof(ConnectionStringType.DefaultConnection))),
                };

                _connection.Open();
            }

            return _connection;
        }

        /// <summary>
        /// Creates a new database connection without opening it.
        /// </summary>
        /// <returns>A new instance of <see cref="IDbConnection"/>.</returns>
        /// <remarks>
        /// This method creates a new database connection based on the selected <see cref="ConnectionStringType"/>
        /// but does not open it. The caller can choose when to open the connection.
        /// </remarks>
        public IDbConnection GetNewConnection()
        {
            return _connectionStringType switch
            {
                ConnectionStringType.PostgreSqlConnection => new NpgsqlConnection(_configuration.GetConnectionString(nameof(ConnectionStringType.PostgreSqlConnection))),
                ConnectionStringType.SqlServerConnection => new SqlConnection(_configuration.GetConnectionString(nameof(ConnectionStringType.SqlServerConnection))),
                ConnectionStringType.DefaultConnection => new SqlConnection(_configuration.GetConnectionString(nameof(ConnectionStringType.DefaultConnection))),
                _ => new SqlConnection(_configuration.GetConnectionString(nameof(ConnectionStringType.DefaultConnection))),
            };
        }

        /// <summary>
        /// Retrieves the current database connection string and its type.
        /// </summary>
        /// <returns>A tuple containing the connection string and the database type.</returns>
        /// <remarks>
        /// This method returns both the current connection string and the type of database connection (SQL Server or PostgreSQL)
        /// based on the selected <see cref="ConnectionStringType"/>.
        /// </remarks>
        public (string? connectionString, ConnectionStringType dbType) GetConnectionStringAndDbType()
        {
            return _connectionStringType switch
            {
                ConnectionStringType.DefaultConnection => (_configuration.GetConnectionString(nameof(ConnectionStringType.DefaultConnection)), ConnectionStringType.DefaultConnection),
                ConnectionStringType.SqlServerConnection => (_configuration.GetConnectionString(nameof(ConnectionStringType.SqlServerConnection)), ConnectionStringType.SqlServerConnection),
                ConnectionStringType.PostgreSqlConnection => (_configuration.GetConnectionString(nameof(ConnectionStringType.PostgreSqlConnection)), ConnectionStringType.PostgreSqlConnection),
                ConnectionStringType.None => (string.Empty, ConnectionStringType.None),
                _ => default,
            };
        }

        /// <summary>
        /// Sets the connection string type (e.g., SQL Server, PostgreSQL) <see cref="ConnectionStringType"/>.
        /// </summary>
        /// <param name="connectionStringType">The database type to use for creating a connection.</param>
        /// <remarks>
        /// This method allows the caller to set the connection string type, which can be used in subsequent calls
        /// to retrieve the appropriate database connection (either SQL Server or PostgreSQL).
        /// </remarks>
        public void SetConnectionStringType(ConnectionStringType connectionStringType)
        {
            _connectionStringType = connectionStringType;
        }

        /// <summary>
        /// Releases database connections properly to prevent memory leaks.
        /// </summary>
        /// <remarks>
        /// This method ensures that database connections are disposed of properly when no longer needed,
        /// releasing unmanaged resources to avoid memory leaks or connection pool issues.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the database connection if it is open.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose managed resources.</param>
        /// <remarks>
        /// This method is called from <see cref="Dispose()"/> to clean up any resources associated with the
        /// database connection when the object is disposed of.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing && _connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Dispose();
            }

            _disposed = true;
        }
    }
}