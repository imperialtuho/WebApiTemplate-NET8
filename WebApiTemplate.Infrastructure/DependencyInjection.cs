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
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers infrastructure-related services, including database, caching, and API clients.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configuration">Application configuration settings.</param>
        /// <returns>The modified IServiceCollection instance.</returns>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            // Adds HTTP client
            services.AddHttpContextAccessor();
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>()!.HttpContext!.User);
            services.AddHttpClient();

            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Memory cache.
            services.AddMemoryCache();

            // Adds API client services
            services.AddTransient<IIdentityApi, IdentityApi>();

            // Adds SqlConnectionFactory
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            // Adds Repositories.
            services.AddScoped<IExampleRepository, ExampleRepository>();

            return services;
        }
    }
}