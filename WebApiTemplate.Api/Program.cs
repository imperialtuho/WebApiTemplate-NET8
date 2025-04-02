using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WebApiTemplate.Api.Extensions;
using WebApiTemplate.Api.Middlewares.Authentication;
using WebApiTemplate.Api.Middlewares.ExceptionHandler;
using WebApiTemplate.Application;
using WebApiTemplate.Application.Configurations.Settings;
using WebApiTemplate.Domain.Constants;
using WebApiTemplate.Infrastructure;

namespace WebApiTemplate.Api
{
    /// <summary>
    /// The entry point of the application.
    /// Responsible for configuring services, middleware, and running the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Stores the Swagger configuration settings loaded from appsettings.
        /// </summary>
        private static SwaggerSettings Swagger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        protected Program()
        { }

        /// <summary>
        /// The entry point for the application.
        /// It initializes configuration, logging, services, and middleware, then starts the application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        protected static async Task Main(string[] args)
        {
            WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

            string environmentName = Environment.GetEnvironmentVariable(ApplicationConstants.AspNetCoreEnvironment) ?? ApplicationConstants.DefaultEnvironmentName;

            // Create a logger
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            loggerFactory.CreateLogger<Program>().LogInformation("Environment name: {EnvironmentName}", environmentName);

            // Load configuration
            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

            // Retrieve Swagger settings
            Swagger = builder.Configuration.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>()
                    ?? throw new ArgumentException($"{nameof(SwaggerSettings)} is missing in appsettings!");

            // Register application services
            ConfigureServices(builder.Services, builder.Configuration);

            WebApplication? app = builder.Build();

            // Configure middleware pipeline
            ConfigureMiddleware(app, environmentName);

            // Run the application
            await app.RunAsync();
        }

        /// <summary>
        /// Configures and registers application services.
        /// </summary>
        /// <param name="services">The service collection to register dependencies.</param>
        /// <param name="configuration">The application configuration.</param>
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Registers custom services
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices(configuration);
            services.AddApiServices(configuration);
            services.AddAuthenticationServices(configuration);

            // Adds Controllers and API Explorer
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Configures Swagger (OpenAPI)
            services.AddSwaggerGen(options =>
            {
                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date"
                });

                // Applies document filter for API path prefixing
                options.DocumentFilter<PathPrefixInsertDocumentFilter>(Swagger.PrefixPath, Swagger.IsExposed);

                // Adds custom operation filters
                options.OperationFilter<AddCommonParameterOperationFilter>();

                // Defines Swagger documentation metadata
                options.SwaggerDoc(Swagger.Version, new OpenApiInfo
                {
                    Title = Swagger.Title,
                    Version = Swagger.Version,
                    Description = Swagger.Description
                });

                // Configures JWT Bearer authentication for Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });
        }

        /// <summary>
        /// Configures the middleware pipeline for request handling.
        /// </summary>
        /// <param name="app">The web application instance.</param>
        /// <param name="environmentName">The current environment name.</param>
        private static void ConfigureMiddleware(WebApplication app, string environmentName)
        {
            var appSettings = app.Services.GetRequiredService<IOptions<ApplicationSettings>>().Value;

            // Enable Swagger for non-production environments
            if (!appSettings.IsProductionMode)
            {
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"swagger/{Swagger.Version}/swagger.json", Swagger.Title);
                    options.RoutePrefix = string.Empty;
                });
            }

            // Configure global exception handling
            if (!ExceptionHandlerMiddleware.IsProductionEnvironment(app.Environment, environmentName))
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler(ExceptionHandlerMiddleware.CustomExceptionHandlerMiddleware(true, app.Logger));
            }
            else
            {
                app.UseExceptionHandler(ExceptionHandlerMiddleware.CustomExceptionHandlerMiddleware(false, app.Logger));
                app.UseHsts();
            }

            // Enforce HTTPS redirection
            app.UseHttpsRedirection();

            // Configure request routing
            app.UseRouting();

            // Apply authentication and authorization
            app.UseAuthorization();

            // Map API controllers
            app.MapControllers();
        }
    }
}