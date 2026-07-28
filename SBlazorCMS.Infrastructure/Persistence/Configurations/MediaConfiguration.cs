using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.Property(m => m.FileName).HasMaxLength(300).IsRequired();
        builder.Property(m => m.Extension).HasMaxLength(20);
        builder.Property(m => m.MimeType).HasMaxLength(150);
    }
}
