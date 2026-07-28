using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Contents;

public class ContentService(IDbContextFactory<ApplicationDbContext> dbFactory, ILanguageService languageService) : IContentService
{
    public async Task<List<ContentListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();

        return await db.Contents
            .OrderBy(c => c.OrderValue)
            .Select(c => new ContentListItemDto
            {
                Id = c.Id,
                Title = c.Translations.Where(t => t.LanguageId == defaultLangId).Select(t => t.Title).FirstOrDefault() ?? "(بدون عنوان)",
                Status = c.Status,
                PublishDate = c.PublishDate,
                OrderValue = c.OrderValue
            })
            .ToListAsync();
    }

    public async Task<ContentEditDto?> GetForEditAsync(Guid contentId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var content = await db.Contents.Include(c => c.Translations).FirstOrDefaultAsync(c => c.Id == contentId);
        if (content is null)
        {
            return null;
        }

        var dto = new ContentEditDto
        {
            Status = content.Status,
            PublishDate = content.PublishDate,
            BigImg = content.BigImg,
            SmallImg = content.SmallImg,
            OrderValue = content.OrderValue,
            SkinId = content.SkinId,
            CategoryIds = await db.ContentCategories.Where(cc => cc.ContentId == contentId).Select(cc => cc.CategoryId).ToListAsync(),
            TagIds = await db.ContentTags.Where(ct => ct.ContentId == contentId).Select(ct => ct.TagId).ToListAsync()
        };

        foreach (var t in content.Translations)
        {
            dto.Translations[t.LanguageId] = new ContentTranslationInput
            {
                Title = t.Title,
                Slug = t.Slug,
                PreTitle = t.PreTitle,
                Summary = t.Summary,
                Body = t.Body,
                Extra = t.Extra,
                SeoTitle = t.SeoTitle,
                SeoDescription = t.SeoDescription
            };
        }

        return dto;
    }

    public async Task<ServiceResult> SaveAsync(ContentSaveRequest request)
    {
        if (request.CurrentUserId is null && request.ContentId is null)
        {
            return ServiceResult.Fail("کاربر نامعتبر است");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();
        if (!request.Translations.TryGetValue(defaultLangId, out var defaultTranslation) ||
            string.IsNullOrWhiteSpace(defaultTranslation.Title) ||
            string.IsNullOrWhiteSpace(defaultTranslation.Slug))
        {
            return ServiceResult.Fail("عنوان و نامک برای زبان پیش‌فرض الزامی است");
        }

        Content content;
        if (request.ContentId is null)
        {
            content = new Content
            {
                Id = Guid.NewGuid(),
                AuthorId = request.CurrentUserId!.Value,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Contents.Add(content);
        }
        else
        {
            var existing = await db.Contents.Include(c => c.Translations).FirstOrDefaultAsync(c => c.Id == request.ContentId);
            if (existing is null)
            {
                return ServiceResult.Fail("محتوا یافت نشد");
            }

            content = existing;
            content.UpdatedAt = DateTime.UtcNow;
            content.UpdatedBy = request.CurrentUserId;
        }

        content.Status = request.Status;
        content.PublishDate = request.PublishDate;
        content.BigImg = request.BigImg;
        content.SmallImg = request.SmallImg;
        content.OrderValue = request.OrderValue;
        content.SkinId = request.SkinId;

        foreach (var (languageId, input) in request.Translations)
        {
            var existingTranslation = content.Translations.FirstOrDefault(t => t.LanguageId == languageId);

            if (string.IsNullOrWhiteSpace(input.Title))
            {
                if (existingTranslation is not null)
                {
                    content.Translations.Remove(existingTranslation);
                    db.Remove(existingTranslation);
                }
                continue;
            }

            if (existingTranslation is null)
            {
                content.Translations.Add(new ContentTranslation
                {
                    Id = Guid.NewGuid(),
                    LanguageId = languageId,
                    Title = input.Title,
                    Slug = input.Slug,
                    PreTitle = input.PreTitle,
                    Summary = input.Summary,
                    Body = input.Body,
                    Extra = input.Extra,
                    SeoTitle = input.SeoTitle,
                    SeoDescription = input.SeoDescription
                });
            }
            else
            {
                existingTranslation.Title = input.Title;
                existingTranslation.Slug = input.Slug;
                existingTranslation.PreTitle = input.PreTitle;
                existingTranslation.Summary = input.Summary;
                existingTranslation.Body = input.Body;
                existingTranslation.Extra = input.Extra;
                existingTranslation.SeoTitle = input.SeoTitle;
                existingTranslation.SeoDescription = input.SeoDescription;
            }
        }

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("خطا در ذخیره‌سازی. ممکن است نامک تکراری باشد.");
        }

        await SyncJunctionAsync(db, db.ContentCategories, content.Id, request.CategoryIds,
            (contentId, categoryId) => new ContentCategory { ContentId = contentId, CategoryId = categoryId },
            cc => cc.CategoryId);

        await SyncJunctionAsync(db, db.ContentTags, content.Id, request.TagIds,
            (contentId, tagId) => new ContentTag { ContentId = contentId, TagId = tagId },
            ct => ct.TagId);

        await db.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    private static async Task SyncJunctionAsync<TJunction>(
        ApplicationDbContext db,
        DbSet<TJunction> set,
        Guid contentId,
        List<Guid> desiredIds,
        Func<Guid, Guid, TJunction> factory,
        Func<TJunction, Guid> relatedIdSelector)
        where TJunction : class
    {
        var existing = await set.Where(j => EF.Property<Guid>(j, "ContentId") == contentId).ToListAsync();
        var existingIds = existing.Select(relatedIdSelector).ToHashSet();
        var desiredSet = desiredIds.ToHashSet();

        var toRemove = existing.Where(j => !desiredSet.Contains(relatedIdSelector(j)));
        set.RemoveRange(toRemove);

        var toAdd = desiredSet.Where(id => !existingIds.Contains(id));
        foreach (var id in toAdd)
        {
            set.Add(factory(contentId, id));
        }
    }

    public async Task<ServiceResult> DeleteAsync(Guid contentId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var content = await db.Contents.FindAsync(contentId);
        if (content is null)
        {
            return ServiceResult.Fail("محتوا یافت نشد");
        }

        content.IsDeleted = true;
        content.DeletedAt = DateTime.UtcNow;
        content.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
