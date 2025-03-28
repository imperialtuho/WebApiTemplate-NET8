using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiTemplate.Domain.Entities;

namespace WebApiTemplate.Infrastructure.Configurations
{
    /// <summary>
    /// Configures the database mapping for the ExampleEntity using Fluent API.
    /// </summary>
    public class ExampleConfiguration : IEntityTypeConfiguration<ExampleEntity>
    {
        /// <summary>
        /// Configures the ExampleEntity mapping to the database.
        /// </summary>
        /// <param name="builder">The entity type builder for ExampleEntity.</param>
        public void Configure(EntityTypeBuilder<ExampleEntity> builder)
        {
            builder.ToTable(nameof(ExampleEntity));
        }
    }
}