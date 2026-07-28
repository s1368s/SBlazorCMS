using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class MenuItemTranslationConfiguration : IEntityTypeConfiguration<MenuItemTranslation>
{
    public void Configure(EntityTypeBuilder<MenuItemTranslation> builder)
    {
        builder.Property(t => t.Title).HasMaxLength(150).IsRequired();

        builder.HasOne(t => t.Language)
            .WithMany()
            .HasForeignKey(t => t.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.MenuItemId, t.LanguageId }).IsUnique();

        builder.HasQueryFilter(t => !t.MenuItem!.IsDeleted && !t.Language!.IsDeleted);
    }
}
