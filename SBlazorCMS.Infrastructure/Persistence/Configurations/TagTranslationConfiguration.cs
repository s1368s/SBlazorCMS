using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class TagTranslationConfiguration : IEntityTypeConfiguration<TagTranslation>
{
    public void Configure(EntityTypeBuilder<TagTranslation> builder)
    {
        builder.Property(t => t.Title).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(150).IsRequired();

        builder.HasOne(t => t.Language)
            .WithMany()
            .HasForeignKey(t => t.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.TagId, t.LanguageId }).IsUnique();
        builder.HasIndex(t => new { t.Slug, t.LanguageId }).IsUnique();

        builder.HasQueryFilter(t => !t.Tag!.IsDeleted && !t.Language!.IsDeleted);
    }
}
