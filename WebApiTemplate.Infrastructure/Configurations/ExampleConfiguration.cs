using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiTemplate.Domain.Entities;

namespace WebApiTemplate.Infrastructure.Configurations
{
    /// <summary>
    /// Configures the database mapping for the <see cref="ExampleEntity"/> using Fluent API.
    /// This class is used to define how the <see cref="ExampleEntity"/> is mapped to the database,
    /// including table name, column names, relationships, and other configurations.
    /// </summary>
    public class ExampleConfiguration : IEntityTypeConfiguration<ExampleEntity>
    {
        /// <summary>
        /// Configures the <see cref="ExampleEntity"/> mapping to the database.
        /// This method is invoked by Entity Framework during model building to configure the
        /// entity's properties and relationships using Fluent API.
        /// </summary>
        /// <param name="builder">The entity type builder for <see cref="ExampleEntity"/>. This builder is used to configure
        /// the entity's table, columns, keys, relationships, etc.</param>
        /// <remarks>
        /// This implementation maps the <see cref="ExampleEntity"/> to the table with the name "ExampleEntity" in the database.
        /// Additional configurations can be added here to fine-tune the mapping, such as setting primary keys,
        /// foreign keys, column constraints, indexes, and more.
        /// </remarks>
        public void Configure(EntityTypeBuilder<ExampleEntity> builder)
        {
            // Maps the ExampleEntity class to a table with the name "ExampleEntity"
            builder.ToTable(nameof(ExampleEntity));
        }
    }
}