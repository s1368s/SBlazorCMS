using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class ContentTranslationConfiguration : IEntityTypeConfiguration<ContentTranslation>
{
    public void Configure(EntityTypeBuilder<ContentTranslation> builder)
    {
        builder.Property(t => t.Title).HasMaxLength(300).IsRequired();
        builder.Property(t => t.Slug).HasMaxLength(300).IsRequired();

        builder.HasOne(t => t.Language)
            .WithMany()
            .HasForeignKey(t => t.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.ContentId, t.LanguageId }).IsUnique();
        builder.HasIndex(t => new { t.Slug, t.LanguageId }).IsUnique();

        builder.HasQueryFilter(t => !t.Content!.IsDeleted && !t.Language!.IsDeleted);
    }
}
