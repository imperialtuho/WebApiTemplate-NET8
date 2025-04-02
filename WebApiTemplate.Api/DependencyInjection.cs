using Asp.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using WebApiTemplate.Domain.Constants;

namespace WebApiTemplate.Api
{
    /// <summary>
    /// Provides extension methods for configuring API-related services in the application's dependency injection container.
    /// </summary>
    /// <remarks>
    /// The <see cref="DependencyInjection"/> class contains methods to configure various API services such as CORS,
    /// request compression, and API versioning within the dependency injection container. These services are essential
    /// for the API's functionality and provide features like cross-origin requests, response compression, and API version management.
    /// </remarks>
    public static class DependencyInjection
    {
        /// <summary>
        /// Configures API-related services such as CORS, request compression, and API versioning.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">Application configuration settings.</param>
        /// <returns>The modified <see cref="IServiceCollection"/> instance with API services added.</returns>
        /// <remarks>
        /// This method sets up several services required for the API's proper operation, including:
        /// - **CORS**: Allows cross-origin requests with the specified policy.
        /// - **Request Compression**: Enables Gzip and Brotli compression for responses, enhancing performance.
        /// - **API Versioning**: Configures API versioning and the corresponding version reader, along with integration with Swagger.
        /// </remarks>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            const int MaxRequestBodySize = 100000000; // Maximum allowed request body size in bytes
            string _myAllowSpecificOrigins = ApplicationConstants.MyAllowSpecificOrigins;

            // Adds CORS support with a policy to allow any origin, method, and header.
            services.AddCors(options =>
            {
                options.AddPolicy(_myAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // Configures IIS server options, specifically the maximum allowed request body size.
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = MaxRequestBodySize;
            });

            // Configures response compression to improve performance by enabling Brotli and Gzip compression for HTTPS requests.
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>(); // Adds Brotli compression
                options.Providers.Add<GzipCompressionProvider>(); // Adds Gzip compression
            });

            // Configures API versioning with default settings, including version reporting and reader for query string.
            IApiVersioningBuilder apiVersioningBuilder = services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); // Default API version, also affects Swagger documentation.
                options.AssumeDefaultVersionWhenUnspecified = true; // Assumes default version when version is not specified in the request.
                options.ReportApiVersions = true; // Reports the available API versions in response headers.
                options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), // Allows versioning via the URL segment (e.g., /api/v1/endpoint).
                    new HeaderApiVersionReader("X-Api-Version")); // Allows clients to specify the version in the request headers using X-Api-Version: {version}.
            });

            // Configures API version explorer for Swagger, allowing versioning to be reflected in the Swagger UI.
            apiVersioningBuilder.AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // Custom format for version grouping in Swagger.
                options.SubstituteApiVersionInUrl = true; // Substitutes version in the URL path, e.g., /api/v1/resource.
            });

            return services; // Returns the modified IServiceCollection instance with added API services.
        }
    }
}