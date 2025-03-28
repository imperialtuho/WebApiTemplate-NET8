using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApiTemplate.Domain.Entities;

namespace WebApiTemplate.Infrastructure.Database
{
    /// <summary>
    /// Represents the application's Entity Framework Core database context.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the database table for ExampleEntity.
        /// </summary>
        public DbSet<ExampleEntity> Examples { get; set; }

        /// <summary>
        /// Default constructor for ApplicationDbContext.
        /// </summary>
        public ApplicationDbContext()
        { }

        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext with database options.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures the entity model using Fluent API.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure entity relationships.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}