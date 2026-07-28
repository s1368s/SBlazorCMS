using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class CategoryTranslationConfiguration : IEntityTypeConfiguration<CategoryTranslation>
{
    public void Configure(EntityTypeBuilder<CategoryTranslation> builder)
    {
        builder.Property(t => t.Title).HasMaxLength(300).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(300).IsRequired();

        builder.HasOne(t => t.Language)
            .WithMany()
            .HasForeignKey(t => t.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.CategoryId, t.LanguageId }).IsUnique();
        builder.HasIndex(t => new { t.Slug, t.LanguageId }).IsUnique();

        builder.HasQueryFilter(t => !t.Category!.IsDeleted && !t.Language!.IsDeleted);
    }
}
