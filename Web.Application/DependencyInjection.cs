using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Principal;
using Web.Application.Configurations.MappingProfiles.Mapster;
using Web.Application.Configurations.Settings;
using Web.Application.Interfaces.Services;
using Web.Application.Services;
using Web.Domain.Constants;

namespace Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Adds setting json
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));

            const int MaxRequestBodySize = 100000000;
            string _myAllowSpecificOrigins = ApplicationConstants.MyAllowSpecificOrigins;
            // Adds system services
            services.AddHttpContextAccessor();

            // Dependency injection support for Mapster
            // https://github.com/MapsterMapper/Mapster/wiki/Dependency-Injection
            var config = new TypeAdapterConfig();
            config.Apply(new MappingRegistration());
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddHttpContextAccessor();
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>()!.HttpContext!.User);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(_myAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = MaxRequestBodySize;
            });

            // Adds application services

            services.AddScoped<IBlogService, BlogService>();

            return services;
        }
    }
}