using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.UserName).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Mobile).HasMaxLength(20);

        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasData(new User
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            FirstName = "مدیر",
            LastName = "سیستم",
            UserName = "admin",
            Email = "admin@sblazorcms.local",
            Mobile = string.Empty,
            // Seed password: Admin@12345 (change after first login)
            PasswordHash = "AQAAAAIAAYagAAAAEKFjM8sxJD86pt62e6NvfOu7OcmnRqcoiGbnSxLT976M+OZVGOtAAmENKEoXLSdu2g==",
            IsActive = true,
            EmailConfirmed = true,
            MobileConfirmed = false,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
