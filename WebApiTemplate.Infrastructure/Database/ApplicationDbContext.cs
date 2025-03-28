using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApiTemplate.Domain.Entities;

namespace WebApiTemplate.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ExampleEntity> Examples { get; set; }

        public ApplicationDbContext()
        { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}