using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;
using WebApiTemplate.Application.Configurations.Database;
using WebApiTemplate.Application.Interfaces.ExternalProviders;
using WebApiTemplate.Application.Interfaces.Repositories;
using WebApiTemplate.Infrastructure.Configurations;
using WebApiTemplate.Infrastructure.Database;
using WebApiTemplate.Infrastructure.Repositories.ExternalProviders.IdentityApi;
using WebApiTemplate.Infrastructure.Repositories.Providers.Example;

namespace WebApiTemplate.Infrastructure
{
    /// <summary>
    /// Provides extension methods for configuring infrastructure services in the application's dependency injection container.
    /// These services include database configuration, HTTP client services, caching, and repositories for external API interaction.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers infrastructure-related services, including database, caching, and API clients.
        /// This method is called during the application startup to set up necessary dependencies.
        /// It configures the database context, HTTP clients, and repositories, making them available for dependency injection
        /// throughout the application.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to. It is used to register dependencies with the DI container.</param>
        /// <param name="configuration">Application configuration settings, typically used to retrieve database connection strings and other settings.</param>
        /// <returns>The modified IServiceCollection instance, which now contains the registered services.</returns>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            // Adds HTTP client and HttpContextAccessor services
            services.AddHttpContextAccessor();
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>()!.HttpContext!.User);
            services.AddHttpClient();

            // Register DbContext with SQL Server connection string
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Registers memory caching services for the application
            services.AddMemoryCache();

            // Registers API client services
            services.AddTransient<IIdentityApi, IdentityApi>();

            // Registers SqlConnectionFactory for database connections
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            // Registers repositories
            services.AddScoped<IExampleRepository, ExampleRepository>();

            return services;
        }
    }
}