using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApiTemplate.Domain.Entities;

namespace WebApiTemplate.Infrastructure.Database
{
    /// <summary>
    /// Represents the application's Entity Framework Core database context.
    /// </summary>
    /// <remarks>
    /// This class serves as the primary interface to the database, inheriting from <see cref="DbContext"/>.
    /// It defines the application's data model and exposes DbSets for each entity type that is part of the
    /// data model, enabling CRUD operations and queries via Entity Framework Core.
    /// </remarks>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the database table for <see cref="ExampleEntity"/>.
        /// </summary>
        /// <remarks>
        /// This property represents the table corresponding to <see cref="ExampleEntity"/> in the database.
        /// The <see cref="DbSet{ExampleEntity}"/> provides access to all CRUD operations for the ExampleEntity.
        /// </remarks>
        public DbSet<ExampleEntity> Examples { get; set; }

        /// <summary>
        /// Default constructor for ApplicationDbContext.
        /// </summary>
        /// <remarks>
        /// This constructor is typically used when the DbContext is configured through dependency injection
        /// and does not require specific options during initialization.
        /// </remarks>
        public ApplicationDbContext()
        { }

        /// <summary>
        /// Initializes a new instance of the ApplicationDbContext with database options.
        /// </summary>
        /// <param name="options">Database context options.</param>
        /// <remarks>
        /// This constructor is used when configuring the DbContext with specific options, such as connection strings
        /// or other configurations required for establishing a connection to the database.
        /// </remarks>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        /// <summary>
        /// Configures the entity model using Fluent API.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure entity relationships.</param>
        /// <remarks>
        /// The method is used to configure the entity model using Fluent API, allowing the customization of
        /// entity relationships, table names, indexes, constraints, etc. The method uses reflection to apply
        /// configurations from the current assembly, making it easier to manage entity configurations in a modular way.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations from the current assembly (mapping configurations, relationships, etc.)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Ensure the base class method is also called for standard initialization
            base.OnModelCreating(modelBuilder);
        }
    }
}