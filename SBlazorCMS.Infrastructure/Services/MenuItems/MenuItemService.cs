using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.ActivityLogs;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.MenuItems;

public class MenuItemService(IDbContextFactory<ApplicationDbContext> dbFactory, ILanguageService languageService, IActivityLogService activityLogService) : IMenuItemService
{
    public async Task<List<MenuItemListItemDto>> GetListByMenuAsync(Guid menuId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();

        var raw = await db.MenuItems
            .Where(i => i.MenuId == menuId)
            .Select(i => new
            {
                i.Id,
                i.Url,
                i.Order,
                i.ParentId,
                Title = i.Translations.Where(t => t.LanguageId == defaultLangId).Select(t => t.Title).FirstOrDefault()
            })
            .OrderBy(i => i.Order)
            .ToListAsync();

        var titleById = raw.ToDictionary(i => i.Id, i => i.Title ?? "(بدون عنوان)");

        return raw.Select(i => new MenuItemListItemDto
        {
            Id = i.Id,
            Title = titleById[i.Id],
            Url = i.Url,
            Order = i.Order,
            ParentTitle = i.ParentId.HasValue && titleById.TryGetValue(i.ParentId.Value, out var pt) ? pt : null
        }).ToList();
    }

    public async Task<List<MenuItemOptionDto>> GetParentOptionsAsync(Guid menuId, Guid? excludeItemId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();

        var allItems = await db.MenuItems
            .Where(i => i.MenuId == menuId)
            .Select(i => new
            {
                i.Id,
                i.ParentId,
                Title = i.Translations.Where(t => t.LanguageId == defaultLangId).Select(t => t.Title).FirstOrDefault()
            })
            .ToListAsync();

        var excluded = new HashSet<Guid>();
        if (excludeItemId is { } editingId)
        {
            excluded.Add(editingId);
            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var i in allItems)
                {
                    if (i.ParentId.HasValue && excluded.Contains(i.ParentId.Value) && excluded.Add(i.Id))
                    {
                        changed = true;
                    }
                }
            }
        }

        return allItems
            .Where(i => !excluded.Contains(i.Id))
            .Select(i => new MenuItemOptionDto(i.Id, i.Title ?? "(بدون عنوان)"))
            .ToList();
    }

    public async Task<MenuItemEditDto?> GetForEditAsync(Guid menuItemId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var item = await db.MenuItems.Include(i => i.Translations).FirstOrDefaultAsync(i => i.Id == menuItemId);
        if (item is null)
        {
            return null;
        }

        var dto = new MenuItemEditDto
        {
            ParentId = item.ParentId,
            Url = item.Url,
            ImgUrl = item.ImgUrl,
            Extra = item.Extra,
            Order = item.Order
        };

        foreach (var t in item.Translations)
        {
            dto.Translations[t.LanguageId] = new MenuItemTranslationInput { Title = t.Title };
        }

        return dto;
    }

    public async Task<ServiceResult> SaveAsync(MenuItemSaveRequest request)
    {
        if (request.ParentId is not null && request.ParentId == request.MenuItemId)
        {
            return ServiceResult.Fail("یک آیتم نمی‌تواند والد خودش باشد");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();
        if (!request.Translations.TryGetValue(defaultLangId, out var defaultTranslation) ||
            string.IsNullOrWhiteSpace(defaultTranslation.Title))
        {
            return ServiceResult.Fail("عنوان برای زبان پیش‌فرض الزامی است");
        }

        MenuItem item;
        if (request.MenuItemId is null)
        {
            item = new MenuItem
            {
                Id = Guid.NewGuid(),
                MenuId = request.MenuId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.MenuItems.Add(item);
        }
        else
        {
            var existing = await db.MenuItems.Include(i => i.Translations).FirstOrDefaultAsync(i => i.Id == request.MenuItemId);
            if (existing is null)
            {
                return ServiceResult.Fail("آیتم یافت نشد");
            }

            item = existing;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = request.CurrentUserId;
        }

        item.ParentId = request.ParentId;
        item.Url = request.Url;
        item.ImgUrl = request.ImgUrl;
        item.Extra = request.Extra;
        item.Order = request.Order;

        foreach (var (languageId, input) in request.Translations)
        {
            var existingTranslation = item.Translations.FirstOrDefault(t => t.LanguageId == languageId);

            if (string.IsNullOrWhiteSpace(input.Title))
            {
                if (existingTranslation is not null)
                {
                    item.Translations.Remove(existingTranslation);
                    db.Remove(existingTranslation);
                }
                continue;
            }

            if (existingTranslation is null)
            {
                item.Translations.Add(new MenuItemTranslation
                {
                    Id = Guid.NewGuid(),
                    LanguageId = languageId,
                    Title = input.Title
                });
            }
            else
            {
                existingTranslation.Title = input.Title;
            }
        }

        var isNew = request.MenuItemId is null;

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(request.CurrentUserId, isNew ? "Create" : "Update", "MenuItem", item.Id.ToString(), defaultTranslation.Title);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(Guid menuItemId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var hasChildren = await db.MenuItems.AnyAsync(i => i.ParentId == menuItemId);
        if (hasChildren)
        {
            return ServiceResult.Fail("این آیتم زیرمجموعه دارد؛ ابتدا زیرمجموعه‌ها را جابه‌جا یا حذف کنید");
        }

        var item = await db.MenuItems.FindAsync(menuItemId);
        if (item is null)
        {
            return ServiceResult.Fail("آیتم یافت نشد");
        }

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(currentUserId, "Delete", "MenuItem", menuItemId.ToString());
        return ServiceResult.Ok();
    }
}
