using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Application.Configurations.Database;
using Web.Application.Interfaces.Repositories;
using Web.Infrastructure.Configurations;
using Web.Infrastructure.Database;
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
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Web.Api")); // Change assembly following the API project's name.
            });

            // Adds Repositories.
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IBlogRepository, BlogRepository>();

            return services;
        }
    }
}