using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.ActivityLogs;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Users;

public class UserService(IDbContextFactory<ApplicationDbContext> dbFactory, IPasswordHasher<User> passwordHasher, IActivityLogService activityLogService) : IUserService
{
    public async Task<List<UserListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var users = await db.Users.OrderBy(u => u.UserName).ToListAsync();
        var roleLinks = await db.UserRoles
            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.DisplayName })
            .ToListAsync();
        var roleNamesByUser = roleLinks
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => string.Join("، ", g.Select(x => x.DisplayName)));

        return users.Select(u => new UserListItemDto
        {
            Id = u.Id,
            FullName = $"{u.FirstName} {u.LastName}".Trim(),
            UserName = u.UserName,
            Email = u.Email,
            IsActive = u.IsActive,
            RoleNames = roleNamesByUser.TryGetValue(u.Id, out var names) && names.Length > 0 ? names : "—"
        }).ToList();
    }

    public async Task<List<RoleOptionDto>> GetRoleOptionsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Roles
            .OrderBy(r => r.DisplayName)
            .Select(r => new RoleOptionDto(r.Id, r.DisplayName))
            .ToListAsync();
    }

    public async Task<UserEditDto?> GetForEditAsync(Guid userId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return null;
        }

        return new UserEditDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Email = user.Email,
            Mobile = user.Mobile,
            IsActive = user.IsActive,
            RoleIds = await db.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync()
        };
    }

    public async Task<ServiceResult> SaveAsync(UserSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return ServiceResult.Fail("نام کاربری الزامی است");
        }

        if (request.UserId is null && string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult.Fail("رمز عبور برای کاربر جدید الزامی است");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        User user;
        if (request.UserId is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Users.Add(user);
        }
        else
        {
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (existing is null)
            {
                return ServiceResult.Fail("کاربر یافت نشد");
            }

            user = existing;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = request.CurrentUserId;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.UserName = request.UserName;
        user.Email = request.Email;
        user.Mobile = request.Mobile;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        }

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("خطا در ذخیره‌سازی. ممکن است نام کاربری یا ایمیل تکراری باشد.");
        }

        var existingLinks = await db.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
        var existingIds = existingLinks.Select(ur => ur.RoleId).ToHashSet();
        var desiredIds = request.RoleIds.ToHashSet();

        db.UserRoles.RemoveRange(existingLinks.Where(ur => !desiredIds.Contains(ur.RoleId)));
        foreach (var id in desiredIds.Where(id => !existingIds.Contains(id)))
        {
            db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = id });
        }

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(request.CurrentUserId, request.UserId is null ? "Create" : "Update", "User", user.Id.ToString(), user.UserName);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(Guid userId, Guid? currentUserId)
    {
        if (currentUserId == userId)
        {
            return ServiceResult.Fail("امکان حذف حساب کاربری خودتان وجود ندارد");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        var user = await db.Users.FindAsync(userId);
        if (user is null)
        {
            return ServiceResult.Fail("کاربر یافت نشد");
        }

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = currentUserId;

        var roleLinks = await db.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
        db.UserRoles.RemoveRange(roleLinks);

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(currentUserId, "Delete", "User", userId.ToString(), user.UserName);
        return ServiceResult.Ok();
    }
}
