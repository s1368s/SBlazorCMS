using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();

        builder.HasIndex(r => r.Name).IsUnique();

        builder.HasData(new Role
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Admin",
            DisplayName = "مدیر کل",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
