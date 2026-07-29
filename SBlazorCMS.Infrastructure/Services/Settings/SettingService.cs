using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Contracts.Settings;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.ActivityLogs;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Settings;

public class SettingService(IDbContextFactory<ApplicationDbContext> dbFactory, IActivityLogService activityLogService) : ISettingService
{
    public async Task<List<SettingPublicDto>> GetByKeysAsync(List<string> keys)
    {
        if (keys.Count == 0)
        {
            return new List<SettingPublicDto>();
        }

        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Settings
            .Where(s => keys.Contains(s.Key))
            .Select(s => new SettingPublicDto
            {
                Key = s.Key,
                Value = s.Value
            })
            .ToListAsync();
    }

    public async Task<List<SettingListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Settings
            .OrderBy(s => s.Key)
            .Select(s => new SettingListItemDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value
            })
            .ToListAsync();
    }

    public async Task<SettingEditDto?> GetForEditAsync(Guid settingId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var setting = await db.Settings.FirstOrDefaultAsync(s => s.Id == settingId);
        if (setting is null)
        {
            return null;
        }

        return new SettingEditDto
        {
            Key = setting.Key,
            Value = setting.Value
        };
    }

    public async Task<ServiceResult> SaveAsync(SettingSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Key))
        {
            return ServiceResult.Fail("کلید تنظیمات الزامی است");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        Setting setting;
        if (request.SettingId is null)
        {
            setting = new Setting
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Settings.Add(setting);
        }
        else
        {
            var existing = await db.Settings.FirstOrDefaultAsync(s => s.Id == request.SettingId);
            if (existing is null)
            {
                return ServiceResult.Fail("تنظیمات یافت نشد");
            }

            setting = existing;
            setting.UpdatedAt = DateTime.UtcNow;
            setting.UpdatedBy = request.CurrentUserId;
        }

        setting.Key = request.Key;
        setting.Value = request.Value;

        var isNew = request.SettingId is null;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("خطا در ذخیره‌سازی. ممکن است کلید تکراری باشد.");
        }

        await activityLogService.LogAsync(request.CurrentUserId, isNew ? "Create" : "Update", "Setting", setting.Id.ToString(), setting.Key);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(Guid settingId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var setting = await db.Settings.FindAsync(settingId);
        if (setting is null)
        {
            return ServiceResult.Fail("تنظیمات یافت نشد");
        }

        setting.IsDeleted = true;
        setting.DeletedAt = DateTime.UtcNow;
        setting.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(currentUserId, "Delete", "Setting", settingId.ToString(), setting.Key);
        return ServiceResult.Ok();
    }
}
