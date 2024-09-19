using Identity.Application;
using Microsoft.EntityFrameworkCore;
using Web.Api.Middlewares.Authentication;
using Web.Domain.Constants;
using Web.Infrastructure;
using Web.Infrastructure.Database;

namespace Web.Api
{
    public class Program
    {
        protected static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string _environmentName = Environment.GetEnvironmentVariable(ApplicationConstants.AspNetCoreEnvironment) ?? ApplicationConstants.DefaultEnvironmentName;

            // Create a logger
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Environment name: {_environmentName}", _environmentName);

            // Adds environment variables from json
            builder.Configuration.SetBasePath(builder.Environment.ContentRootPath).AddEnvironmentVariables();

            // Add services to the container.
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddJWTServices(builder.Configuration);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                string? assemblyName = typeof(Program).Assembly.GetName().Name;
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(assemblyName));
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}