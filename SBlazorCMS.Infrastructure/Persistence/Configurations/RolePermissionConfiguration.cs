using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);

        var adminRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        builder.HasData(
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000001") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000002") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000003") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000004") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000005") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000006") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000007") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000008") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000009") },
            new RolePermission { RoleId = adminRoleId, PermissionId = Guid.Parse("55555555-5555-5555-5555-000000000010") });
    }
}
