﻿using Microsoft.EntityFrameworkCore;
using Web.Application.Configurations.Database;
using Web.Domain.Enums;
using Web.Infrastructure.Repositories.Providers;

namespace Web.Infrastructure.Configurations
{
    public abstract class DbSqlConnectionEFRepositoryBase<C, T> : EFGenericRepository<C, T>
        where T : class
        where C : DbContext, new()
    {
        protected DbSqlConnectionEFRepositoryBase(ISqlConnectionFactory sqlConnectionFactory) : base(CreateDbContextOptions(sqlConnectionFactory), sqlConnectionFactory)
        {
            sqlConnectionFactory.SetConnectionStringType(ConnectionStringType.DefaultConnection);
        }

        private static DbContextOptions<C> CreateDbContextOptions(ISqlConnectionFactory sqlConnectionFactory)
        {
            (string? connectionString, ConnectionStringType dbType) = sqlConnectionFactory.GetConnectionStringAndDbType();
            var optionsBuilder = new DbContextOptionsBuilder<C>();

            if (connectionString != null)
            {
                switch (dbType)
                {
                    case ConnectionStringType.PostgresqlConnection:
                        optionsBuilder.UseNpgsql(connectionString);
                        break;

                    case ConnectionStringType.SqlServerConnection:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;

                    default:
                        optionsBuilder.UseSqlServer(connectionString);
                        break;
                }
            }

            return optionsBuilder.Options;
        }
    }
}