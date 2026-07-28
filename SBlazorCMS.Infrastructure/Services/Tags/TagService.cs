using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Tags;

public class TagService(IDbContextFactory<ApplicationDbContext> dbFactory, ILanguageService languageService) : ITagService
{
    public async Task<List<TagListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();

        var raw = await db.Tags
            .Select(t => new
            {
                t.Id,
                Translation = t.Translations.Where(tr => tr.LanguageId == defaultLangId).Select(tr => new { tr.Title, tr.Slug }).FirstOrDefault()
            })
            .ToListAsync();

        return raw.Select(t => new TagListItemDto
        {
            Id = t.Id,
            Title = t.Translation?.Title ?? "(بدون عنوان)",
            Slug = t.Translation?.Slug ?? string.Empty
        }).ToList();
    }

    public async Task<TagEditDto?> GetForEditAsync(Guid tagId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var tag = await db.Tags.Include(t => t.Translations).FirstOrDefaultAsync(t => t.Id == tagId);
        if (tag is null)
        {
            return null;
        }

        var dto = new TagEditDto();
        foreach (var t in tag.Translations)
        {
            dto.Translations[t.LanguageId] = new TagTranslationInput
            {
                Title = t.Title,
                Slug = t.Slug
            };
        }

        return dto;
    }

    public async Task<ServiceResult> SaveAsync(TagSaveRequest request)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();
        if (!request.Translations.TryGetValue(defaultLangId, out var defaultTranslation) ||
            string.IsNullOrWhiteSpace(defaultTranslation.Title) ||
            string.IsNullOrWhiteSpace(defaultTranslation.Slug))
        {
            return ServiceResult.Fail("عنوان و نامک برای زبان پیش‌فرض الزامی است");
        }

        Tag tag;
        if (request.TagId is null)
        {
            tag = new Tag
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Tags.Add(tag);
        }
        else
        {
            var existing = await db.Tags.Include(t => t.Translations).FirstOrDefaultAsync(t => t.Id == request.TagId);
            if (existing is null)
            {
                return ServiceResult.Fail("برچسب یافت نشد");
            }

            tag = existing;
            tag.UpdatedAt = DateTime.UtcNow;
            tag.UpdatedBy = request.CurrentUserId;
        }

        foreach (var (languageId, input) in request.Translations)
        {
            var existingTranslation = tag.Translations.FirstOrDefault(t => t.LanguageId == languageId);

            if (string.IsNullOrWhiteSpace(input.Title))
            {
                if (existingTranslation is not null)
                {
                    tag.Translations.Remove(existingTranslation);
                    db.Remove(existingTranslation);
                }
                continue;
            }

            if (existingTranslation is null)
            {
                tag.Translations.Add(new TagTranslation
                {
                    Id = Guid.NewGuid(),
                    LanguageId = languageId,
                    Title = input.Title,
                    Slug = input.Slug
                });
            }
            else
            {
                existingTranslation.Title = input.Title;
                existingTranslation.Slug = input.Slug;
            }
        }

        try
        {
            await db.SaveChangesAsync();
            return ServiceResult.Ok();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("خطا در ذخیره‌سازی. ممکن است نامک تکراری باشد.");
        }
    }

    public async Task<ServiceResult> DeleteAsync(Guid tagId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var tag = await db.Tags.FindAsync(tagId);
        if (tag is null)
        {
            return ServiceResult.Fail("برچسب یافت نشد");
        }

        tag.IsDeleted = true;
        tag.DeletedAt = DateTime.UtcNow;
        tag.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
