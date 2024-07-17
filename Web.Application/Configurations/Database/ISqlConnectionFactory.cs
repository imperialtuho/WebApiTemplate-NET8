using System.Data;
using Web.Domain.Enums;

namespace Web.Application.Configurations.Database
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();

        IDbConnection GetNewConnection();

        void SetConnectionStringType(ConnectionStringType connectionStringType);

        (string? connectionString, ConnectionStringType dbType) GetConnectionStringAndDbType();
    }
}