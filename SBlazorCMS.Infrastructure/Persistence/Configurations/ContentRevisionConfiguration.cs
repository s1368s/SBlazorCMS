using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class ContentRevisionConfiguration : IEntityTypeConfiguration<ContentRevision>
{
    public void Configure(EntityTypeBuilder<ContentRevision> builder)
    {
        builder.Property(r => r.Title).HasMaxLength(300).IsRequired();
        builder.Property(r => r.Slug).HasMaxLength(300).IsRequired();

        builder.HasOne(r => r.Content)
            .WithMany()
            .HasForeignKey(r => r.ContentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Language)
            .WithMany()
            .HasForeignKey(r => r.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.ContentId, r.LanguageId });

        builder.HasQueryFilter(r => !r.Content!.IsDeleted && !r.Language!.IsDeleted);
    }
}
