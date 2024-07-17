using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Domain.Entities;

namespace Web.Infrastructure.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blog");
            //builder.HasMany(menuItem => menuItem.Children)
            //       .WithOne(menuItem => menuItem.ParentItem)
            //       .HasForeignKey(menuItem => menuItem.ParentId);

            // Add RoleMenu configuration
            //builder.HasMany(menuItem => menuItem.RoleMenus)
            //       .WithOne(roleMenu => roleMenu.Menu)
            //       .HasForeignKey(roleMenu => roleMenu.MenuId);

            // Add UserMenu configuration
            //builder.HasMany(menuItem => menuItem.UserMenus)
            //      .WithOne(userMenu => userMenu.Menu)
            //      .HasForeignKey(userMenu => userMenu.MenuId);
        }
    }
}