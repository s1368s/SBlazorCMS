using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.HasOne(c => c.Skin)
            .WithMany()
            .HasForeignKey(c => c.SkinId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Translations)
            .WithOne(t => t.Content)
            .HasForeignKey(t => t.ContentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Status);
    }
}
