using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Categories;

public class CategoryService(IDbContextFactory<ApplicationDbContext> dbFactory, ILanguageService languageService) : ICategoryService
{
    public async Task<List<CategoryListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();

        var raw = await db.Categories
            .Select(c => new
            {
                c.Id,
                c.Code,
                c.OrderValue,
                c.ShowCount,
                c.ParentId,
                Title = c.Translations.Where(t => t.LanguageId == defaultLangId).Select(t => t.Title).FirstOrDefault()
            })
            .OrderBy(c => c.OrderValue)
            .ToListAsync();

        var titleById = raw.ToDictionary(c => c.Id, c => c.Title ?? "(بدون عنوان)");

        return raw.Select(c => new CategoryListItemDto
        {
            Id = c.Id,
            Title = titleById[c.Id],
            Code = c.Code,
            OrderValue = c.OrderValue,
            ShowCount = c.ShowCount,
            ParentTitle = c.ParentId.HasValue && titleById.TryGetValue(c.ParentId.Value, out var pt) ? pt : null
        }).ToList();
    }

    public async Task<List<CategoryOptionDto>> GetParentOptionsAsync(Guid? excludeCategoryId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();

        var allCategories = await db.Categories
            .Select(c => new
            {
                c.Id,
                c.ParentId,
                Title = c.Translations.Where(t => t.LanguageId == defaultLangId).Select(t => t.Title).FirstOrDefault()
            })
            .ToListAsync();

        var excluded = new HashSet<Guid>();
        if (excludeCategoryId is { } editingId)
        {
            excluded.Add(editingId);
            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var c in allCategories)
                {
                    if (c.ParentId.HasValue && excluded.Contains(c.ParentId.Value) && excluded.Add(c.Id))
                    {
                        changed = true;
                    }
                }
            }
        }

        return allCategories
            .Where(c => !excluded.Contains(c.Id))
            .Select(c => new CategoryOptionDto(c.Id, c.Title ?? "(بدون عنوان)"))
            .ToList();
    }

    public async Task<CategoryEditDto?> GetForEditAsync(Guid categoryId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var category = await db.Categories.Include(c => c.Translations).FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category is null)
        {
            return null;
        }

        var dto = new CategoryEditDto
        {
            Code = category.Code,
            ParentId = category.ParentId,
            SkinId = category.SkinId,
            OrderValue = category.OrderValue,
            ShowCount = category.ShowCount
        };

        foreach (var t in category.Translations)
        {
            dto.Translations[t.LanguageId] = new CategoryTranslationInput
            {
                Title = t.Title,
                Slug = t.Slug,
                Description = t.Description
            };
        }

        return dto;
    }

    public async Task<ServiceResult> SaveAsync(CategorySaveRequest request)
    {
        if (request.ParentId is not null && request.ParentId == request.CategoryId)
        {
            return ServiceResult.Fail("یک دسته‌بندی نمی‌تواند والد خودش باشد");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();
        if (!request.Translations.TryGetValue(defaultLangId, out var defaultTranslation) ||
            string.IsNullOrWhiteSpace(defaultTranslation.Title) ||
            string.IsNullOrWhiteSpace(defaultTranslation.Slug))
        {
            return ServiceResult.Fail("عنوان و نامک برای زبان پیش‌فرض الزامی است");
        }

        Category category;
        if (request.CategoryId is null)
        {
            category = new Category
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Categories.Add(category);
        }
        else
        {
            var existing = await db.Categories.Include(c => c.Translations).FirstOrDefaultAsync(c => c.Id == request.CategoryId);
            if (existing is null)
            {
                return ServiceResult.Fail("دسته‌بندی یافت نشد");
            }

            category = existing;
            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = request.CurrentUserId;
        }

        category.Code = request.Code;
        category.ParentId = request.ParentId;
        category.SkinId = request.SkinId;
        category.OrderValue = request.OrderValue;
        category.ShowCount = request.ShowCount;

        foreach (var (languageId, input) in request.Translations)
        {
            var existingTranslation = category.Translations.FirstOrDefault(t => t.LanguageId == languageId);

            if (string.IsNullOrWhiteSpace(input.Title))
            {
                if (existingTranslation is not null)
                {
                    category.Translations.Remove(existingTranslation);
                    db.Remove(existingTranslation);
                }
                continue;
            }

            if (existingTranslation is null)
            {
                category.Translations.Add(new CategoryTranslation
                {
                    Id = Guid.NewGuid(),
                    LanguageId = languageId,
                    Title = input.Title,
                    Slug = input.Slug,
                    Description = input.Description
                });
            }
            else
            {
                existingTranslation.Title = input.Title;
                existingTranslation.Slug = input.Slug;
                existingTranslation.Description = input.Description;
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

    public async Task<ServiceResult> DeleteAsync(Guid categoryId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var hasChildren = await db.Categories.AnyAsync(c => c.ParentId == categoryId);
        if (hasChildren)
        {
            return ServiceResult.Fail("این دسته‌بندی زیرمجموعه دارد؛ ابتدا زیرمجموعه‌ها را جابه‌جا یا حذف کنید");
        }

        var category = await db.Categories.FindAsync(categoryId);
        if (category is null)
        {
            return ServiceResult.Fail("دسته‌بندی یافت نشد");
        }

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        category.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
