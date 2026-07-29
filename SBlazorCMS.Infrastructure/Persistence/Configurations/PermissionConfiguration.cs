using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(p => p.Code).HasMaxLength(100).IsRequired();

        builder.HasIndex(p => p.Code).IsUnique();

        builder.HasData(
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000001"), Code = "content.manage", Name = "مدیریت محتوا" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000002"), Code = "categories.manage", Name = "مدیریت دسته‌بندی‌ها" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000003"), Code = "tags.manage", Name = "مدیریت برچسب‌ها" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000004"), Code = "comments.manage", Name = "مدیریت دیدگاه‌ها" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000005"), Code = "media.manage", Name = "مدیریت رسانه‌ها" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000006"), Code = "menus.manage", Name = "مدیریت منوها" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000007"), Code = "users.manage", Name = "مدیریت کاربران" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000008"), Code = "roles.manage", Name = "مدیریت نقش‌ها" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000009"), Code = "settings.manage", Name = "مدیریت تنظیمات" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000010"), Code = "skins.manage", Name = "مدیریت قالب‌ها" },
            new Permission { Id = Guid.Parse("55555555-5555-5555-5555-000000000011"), Code = "languages.manage", Name = "مدیریت زبان‌ها" });
    }
}
