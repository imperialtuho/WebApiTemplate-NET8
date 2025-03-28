using Asp.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using WebApiTemplate.Domain.Constants;

namespace WebApiTemplate.Api
{
    /// <summary>
    /// Provides extension methods for configuring API-related services in the application's dependency injection container.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Configures API-related services such as CORS, request compression, and API versioning.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configuration">Application configuration settings.</param>
        /// <returns>The modified IServiceCollection instance.</returns>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            const int MaxRequestBodySize = 100000000;
            string _myAllowSpecificOrigins = ApplicationConstants.MyAllowSpecificOrigins;

            // Adds CORS
            services.AddCors(options =>
            {
                options.AddPolicy(_myAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // IIS Server options
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = MaxRequestBodySize;
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            // Adds versioning
            IApiVersioningBuilder apiVersioningBuilder = services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); // Config valid API version - This affects Swagger API version too.
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
            });

            // Adds API version explorer for Swagger
            apiVersioningBuilder.AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}