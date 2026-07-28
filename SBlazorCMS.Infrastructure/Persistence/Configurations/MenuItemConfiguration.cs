using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.Property(i => i.Url).HasMaxLength(500);

        builder.HasOne(i => i.Parent)
            .WithMany(i => i.Children)
            .HasForeignKey(i => i.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Translations)
            .WithOne(t => t.MenuItem)
            .HasForeignKey(t => t.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
