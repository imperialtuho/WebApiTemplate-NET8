using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Application.Configurations.Database;
using Web.Application.Interfaces.Repositories;
using Web.Infrastructure.Configurations;
using Web.Infrastructure.Repositories.Providers.Blogs;

namespace Web.Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds Infrastructure Services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Adds Repositories.
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IBlogRepository, BlogRepository>();

            return services;
        }
    }
}