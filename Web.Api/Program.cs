using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Web.Api.Extensions;
using Web.Api.Middlewares.Authentication;
using Web.Api.Middlewares.ExceptionHandler;
using Web.Application;
using Web.Application.Configurations.Settings;
using Web.Domain.Constants;
using Web.Infrastructure;

namespace Web.Api
{
    public class Program
    {
        private static SwaggerSettings Swagger;

        protected Program()
        { }

        protected static async Task Main(string[] args)
        {
            var environmentName = ApplicationConstants.EnvironmentName;

            var builder = WebApplication.CreateBuilder(args);

            environmentName = Environment.GetEnvironmentVariable(ApplicationConstants.AspNetCoreEnvironment) ?? ApplicationConstants.DefaultEnvironmentName;

            // Create a logger
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            loggerFactory.CreateLogger<Program>().LogInformation("Environment name: {EnvironmentName}", environmentName);

            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

            Swagger = builder.Configuration.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>()
                    ?? throw new ArgumentException($"{nameof(SwaggerSettings)} is missing in appsettings!");

            // Register services
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure middleware and request pipeline
            ConfigureMiddleware(app, environmentName);

            await app.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register custom services
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices(configuration);
            services.AddApiServices(configuration);
            services.AddJwtServices(configuration);

            // Add Controllers and Swagger
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Adds Swagger
            services.AddSwaggerGen(options =>
            {
                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date"
                });

                // Apply document filter by exposed or add prefix
                options.DocumentFilter<PathPrefixInsertDocumentFilter>(Swagger.PrefixPath, Swagger.IsExposed);

                // Sepcify our operation filter here
                options.OperationFilter<AddCommonParameterOperationFilter>();

                options.SwaggerDoc(Swagger.Version, new OpenApiInfo
                {
                    Title = Swagger.Title,
                    Version = Swagger.Version,
                    Description = Swagger.Description
                });

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

        private static void ConfigureMiddleware(WebApplication app, string environmentName)
        {
            var appSettings = app.Services.GetRequiredService<IOptions<ApplicationSettings>>().Value;

            if (!appSettings.IsProductionMode)
            {
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"swagger/{Swagger.Version}/swagger.json", Swagger.Title);
                    options.RoutePrefix = string.Empty;
                });
            }

            // Configure error handling
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

            // Configure HTTPS redirection and security middleware
            app.UseHttpsRedirection();

            // Configure routing
            app.UseRouting();

            // Authentication and Authorization
            app.UseAuthorization();

            // Map routes
            app.MapControllers();
        }
    }
}