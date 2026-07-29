using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.ActivityLogs;

namespace SBlazorCMS.Infrastructure.Services.Common;

public class LanguageService(IDbContextFactory<ApplicationDbContext> dbFactory, IActivityLogService activityLogService) : ILanguageService
{
    public async Task<List<LanguageDto>> GetActiveLanguagesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Languages
            .Where(l => l.IsActive)
            .OrderByDescending(l => l.IsDefault)
            .Select(l => new LanguageDto(l.Id, l.Code, l.Name, l.IsDefault))
            .ToListAsync();
    }

    public async Task<Guid> GetDefaultLanguageIdAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Languages.Where(l => l.IsDefault).Select(l => l.Id).FirstAsync();
    }

    public async Task<List<LanguageListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Languages
            .OrderByDescending(l => l.IsDefault)
            .ThenBy(l => l.Name)
            .Select(l => new LanguageListItemDto
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                IsDefault = l.IsDefault,
                IsActive = l.IsActive
            })
            .ToListAsync();
    }

    public async Task<LanguageEditDto?> GetForEditAsync(Guid languageId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var language = await db.Languages.FirstOrDefaultAsync(l => l.Id == languageId);
        if (language is null)
        {
            return null;
        }

        return new LanguageEditDto
        {
            Code = language.Code,
            Name = language.Name,
            IsDefault = language.IsDefault,
            IsActive = language.IsActive
        };
    }

    public async Task<ServiceResult> SaveAsync(LanguageSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult.Fail("کد و نام زبان الزامی است");
        }

        if (request.IsDefault && !request.IsActive)
        {
            return ServiceResult.Fail("زبان پیش‌فرض نمی‌تواند غیرفعال باشد");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        Language language;
        if (request.LanguageId is null)
        {
            language = new Language
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Languages.Add(language);
        }
        else
        {
            var existing = await db.Languages.FirstOrDefaultAsync(l => l.Id == request.LanguageId);
            if (existing is null)
            {
                return ServiceResult.Fail("زبان یافت نشد");
            }

            if (existing.IsDefault && !request.IsDefault)
            {
                return ServiceResult.Fail("باید حداقل یک زبان پیش‌فرض وجود داشته باشد؛ ابتدا زبان دیگری را پیش‌فرض کنید");
            }

            language = existing;
            language.UpdatedAt = DateTime.UtcNow;
            language.UpdatedBy = request.CurrentUserId;
        }

        language.Code = request.Code;
        language.Name = request.Name;
        language.IsDefault = request.IsDefault;
        language.IsActive = request.IsActive;

        if (request.IsDefault)
        {
            var others = await db.Languages.Where(l => l.IsDefault && l.Id != language.Id).ToListAsync();
            foreach (var other in others)
            {
                other.IsDefault = false;
            }
        }

        var isNew = request.LanguageId is null;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("خطا در ذخیره‌سازی. ممکن است کد زبان تکراری باشد.");
        }

        await activityLogService.LogAsync(request.CurrentUserId, isNew ? "Create" : "Update", "Language", language.Id.ToString(), language.Name);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(Guid languageId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var language = await db.Languages.FindAsync(languageId);
        if (language is null)
        {
            return ServiceResult.Fail("زبان یافت نشد");
        }

        if (language.IsDefault)
        {
            return ServiceResult.Fail("زبان پیش‌فرض را نمی‌توان حذف کرد؛ ابتدا زبان دیگری را پیش‌فرض کنید");
        }

        var inUse = await db.ContentTranslations.AnyAsync(t => t.LanguageId == languageId)
            || await db.CategoryTranslations.AnyAsync(t => t.LanguageId == languageId)
            || await db.TagTranslations.AnyAsync(t => t.LanguageId == languageId)
            || await db.MenuItemTranslations.AnyAsync(t => t.LanguageId == languageId)
            || await db.ContentRevisions.AnyAsync(r => r.LanguageId == languageId);

        if (inUse)
        {
            return ServiceResult.Fail("این زبان در محتوای موجود استفاده شده است؛ نمی‌توان آن را حذف کرد");
        }

        language.IsDeleted = true;
        language.DeletedAt = DateTime.UtcNow;
        language.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(currentUserId, "Delete", "Language", languageId.ToString(), language.Name);
        return ServiceResult.Ok();
    }
}
