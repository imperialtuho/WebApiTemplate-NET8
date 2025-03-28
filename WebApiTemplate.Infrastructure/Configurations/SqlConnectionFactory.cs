using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Domain.Enums;

namespace WebApiTemplate.Infrastructure.Configurations
{
    /// <summary>
    /// Factory for managing database connections, supporting both SQL Server and PostgreSQL.
    /// </summary>
    public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IDbConnection? _connection;
        private bool _disposed = false;
        private ConnectionStringType _connectionStringType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration settings.</param>
        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gets an open database connection. If a connection is already open, it returns that.
        /// </summary>
        /// <returns>An open <see cref="IDbConnection"/> instance.</returns>
        public IDbConnection GetOpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                switch (_connectionStringType)
                {
                    case ConnectionStringType.PostgresqlConnection:
                        _connection = new NpgsqlConnection(_configuration.GetConnectionString("PostgresqlConnection"));
                        break;

                    case ConnectionStringType.SqlServerConnection:
                        _connection = new SqlConnection(_configuration.GetConnectionString("SqlServerConnection"));
                        break;

                    case ConnectionStringType.DefaultConnection:
                        _connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                        break;

                    default:
                        _connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                        break;
                }

                _connection.Open();
            }

            return _connection;
        }

        /// <summary>
        /// Creates a new database connection without opening it.
        /// </summary>
        /// <returns>A new instance of <see cref="IDbConnection"/>.</returns>
        public IDbConnection GetNewConnection()
        {
            switch (_connectionStringType)
            {
                case ConnectionStringType.PostgresqlConnection:
                    return new NpgsqlConnection(_configuration.GetConnectionString("PostgresqlConnection"));

                case ConnectionStringType.SqlServerConnection:
                    return new SqlConnection(_configuration.GetConnectionString("SqlServerConnection"));

                case ConnectionStringType.DefaultConnection:
                    return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                default:
                    return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        /// <summary>
        /// Retrieves the current database connection string and its type.
        /// </summary>
        /// <returns>A tuple containing the connection string and the database type.</returns>
        public (string? connectionString, ConnectionStringType dbType) GetConnectionStringAndDbType()
        {
            switch (_connectionStringType)
            {
                case ConnectionStringType.DefaultConnection:
                    return (_configuration.GetConnectionString("DefaultConnection"), ConnectionStringType.DefaultConnection);

                case ConnectionStringType.SqlServerConnection:
                    return (_configuration.GetConnectionString("SqlServerConnection"), ConnectionStringType.SqlServerConnection);

                case ConnectionStringType.PostgresqlConnection:
                    return (_configuration.GetConnectionString("PostgresqlConnection"), ConnectionStringType.PostgresqlConnection);

                case ConnectionStringType.None:
                    return (string.Empty, ConnectionStringType.None);

                default:
                    return default;
            }
        }

        /// <summary>
        /// Sets the connection string type (e.g., SQL Server, PostgreSQL) <see cref="ConnectionStringType"/>.
        /// </summary>
        /// <param name="connectionStringType">The database type to use.</param>
        public void SetConnectionStringType(ConnectionStringType connectionStringType)
        {
            _connectionStringType = connectionStringType;
        }

        /// <summary>
        /// Releases database connections properly to prevent memory leaks.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the database connection if it is open.
        /// </summary>
        /// <param name="disposing">Indicates whether to dispose managed resources.</param>
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