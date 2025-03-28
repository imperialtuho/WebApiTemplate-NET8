using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WebApiTemplate.Application.Configurations.MappingProfiles.Mapster;
using WebApiTemplate.Application.Configurations.Settings;
using WebApiTemplate.Application.Interfaces.Services;
using WebApiTemplate.Application.Services;

namespace WebApiTemplate.Application
{
    /// <summary>
    /// Provides extension methods for configuring infrastructure services in the application's dependency injection container.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Configures application-wide services, settings, and dependency injection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configuration">Application configuration settings.</param>
        /// <returns>The modified IServiceCollection instance.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Gets appsetting json section details
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));
            services.Configure<IdentityApiSettings>(configuration.GetSection(nameof(IdentityApiSettings)));

            // Dependency injection support for Mapster
            // https://github.com/MapsterMapper/Mapster/wiki/Dependency-Injection
            var config = TypeAdapterConfig.GlobalSettings;
            config.Apply(new MappingRegistration());
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Adds application services
            services.AddScoped<IExampleService, ExampleService>();

            return services;
        }
    }
}