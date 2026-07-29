using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Roles;

public class RoleService(IDbContextFactory<ApplicationDbContext> dbFactory) : IRoleService
{
    public async Task<List<RoleListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Roles
            .OrderBy(r => r.DisplayName)
            .Select(r => new RoleListItemDto
            {
                Id = r.Id,
                Name = r.Name,
                DisplayName = r.DisplayName,
                PermissionCount = db.RolePermissions.Count(rp => rp.RoleId == r.Id),
                UserCount = db.UserRoles.Count(ur => ur.RoleId == r.Id)
            })
            .ToListAsync();
    }

    public async Task<List<PermissionOptionDto>> GetPermissionOptionsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Permissions
            .OrderBy(p => p.Name)
            .Select(p => new PermissionOptionDto(p.Id, p.Code, p.Name))
            .ToListAsync();
    }

    public async Task<RoleEditDto?> GetForEditAsync(Guid roleId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
        if (role is null)
        {
            return null;
        }

        return new RoleEditDto
        {
            Name = role.Name,
            DisplayName = role.DisplayName,
            PermissionIds = await db.RolePermissions.Where(rp => rp.RoleId == roleId).Select(rp => rp.PermissionId).ToListAsync()
        };
    }

    public async Task<ServiceResult> SaveAsync(RoleSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return ServiceResult.Fail("نام و عنوان نمایشی نقش الزامی است");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        Role role;
        if (request.RoleId is null)
        {
            role = new Role
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Roles.Add(role);
        }
        else
        {
            var existing = await db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId);
            if (existing is null)
            {
                return ServiceResult.Fail("نقش یافت نشد");
            }

            role = existing;
            role.UpdatedAt = DateTime.UtcNow;
            role.UpdatedBy = request.CurrentUserId;
        }

        role.Name = request.Name;
        role.DisplayName = request.DisplayName;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("خطا در ذخیره‌سازی. ممکن است نام نقش تکراری باشد.");
        }

        var existingLinks = await db.RolePermissions.Where(rp => rp.RoleId == role.Id).ToListAsync();
        var existingIds = existingLinks.Select(rp => rp.PermissionId).ToHashSet();
        var desiredIds = request.PermissionIds.ToHashSet();

        db.RolePermissions.RemoveRange(existingLinks.Where(rp => !desiredIds.Contains(rp.PermissionId)));
        foreach (var id in desiredIds.Where(id => !existingIds.Contains(id)))
        {
            db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = id });
        }

        await db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(Guid roleId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var hasUsers = await db.UserRoles.AnyAsync(ur => ur.RoleId == roleId);
        if (hasUsers)
        {
            return ServiceResult.Fail("این نقش به کاربرانی اختصاص یافته است؛ ابتدا آن‌ها را از این نقش خارج کنید");
        }

        var role = await db.Roles.FindAsync(roleId);
        if (role is null)
        {
            return ServiceResult.Fail("نقش یافت نشد");
        }

        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;
        role.DeletedBy = currentUserId;

        var permissionLinks = await db.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        db.RolePermissions.RemoveRange(permissionLinks);

        await db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
