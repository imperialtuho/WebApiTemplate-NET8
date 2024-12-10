using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Web.Application.Configurations.MappingProfiles.Mapster;
using Web.Application.Configurations.Settings;
using Web.Application.Interfaces.Services;
using Web.Application.Services;

namespace Web.Application
{
    public static class DependencyInjection
    {
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