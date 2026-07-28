using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.Property(l => l.Code).HasMaxLength(10).IsRequired();
        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();

        builder.HasIndex(l => l.Code).IsUnique();

        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new Language
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Code = "fa",
                Name = "فارسی",
                IsDefault = true,
                IsActive = true,
                CreatedAt = seedDate
            },
            new Language
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Code = "en",
                Name = "English",
                IsDefault = false,
                IsActive = true,
                CreatedAt = seedDate
            });
    }
}
