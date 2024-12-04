using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;
using Web.Application.Configurations.Database;
using Web.Application.Interfaces.ExternalProviders;
using Web.Application.Interfaces.Repositories;
using Web.Infrastructure.Configurations;
using Web.Infrastructure.Repositories.ExternalProviders.IdentityApi;
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
            services.AddOptions();

            services.AddHttpContextAccessor();
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>()!.HttpContext!.User);
            services.AddHttpClient();

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