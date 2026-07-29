using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Contracts.Menus;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Menus;

public class MenuService(IDbContextFactory<ApplicationDbContext> dbFactory, ILanguageService languageService) : IMenuService
{
    public async Task<List<MenuListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Menus
            .OrderBy(m => m.Name)
            .Select(m => new MenuListItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Location = m.Location,
                ItemCount = m.Items.Count
            })
            .ToListAsync();
    }

    public async Task<string?> GetNameAsync(Guid menuId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Menus.Where(m => m.Id == menuId).Select(m => m.Name).FirstOrDefaultAsync();
    }

    public async Task<MenuEditDto?> GetForEditAsync(Guid menuId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var menu = await db.Menus.FirstOrDefaultAsync(m => m.Id == menuId);
        if (menu is null)
        {
            return null;
        }

        return new MenuEditDto
        {
            Name = menu.Name,
            Description = menu.Description,
            Location = menu.Location
        };
    }

    public async Task<ServiceResult> SaveAsync(MenuSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ServiceResult.Fail("نام منو الزامی است");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        Menu menu;
        if (request.MenuId is null)
        {
            menu = new Menu
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Menus.Add(menu);
        }
        else
        {
            var existing = await db.Menus.FirstOrDefaultAsync(m => m.Id == request.MenuId);
            if (existing is null)
            {
                return ServiceResult.Fail("منو یافت نشد");
            }

            menu = existing;
            menu.UpdatedAt = DateTime.UtcNow;
            menu.UpdatedBy = request.CurrentUserId;
        }

        menu.Name = request.Name;
        menu.Description = request.Description;
        menu.Location = request.Location;

        try
        {
            await db.SaveChangesAsync();
            return ServiceResult.Ok();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail("خطا در ذخیره‌سازی. ممکن است نام منو تکراری باشد.");
        }
    }

    public async Task<MenuPublicDto?> GetPublicByNameAsync(string name, string? languageCode)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var menu = await db.Menus
            .Include(m => m.Items)
                .ThenInclude(i => i.Translations)
            .FirstOrDefaultAsync(m => m.Name == name);

        if (menu is null)
        {
            return null;
        }

        var languageId = string.IsNullOrWhiteSpace(languageCode)
            ? await languageService.GetDefaultLanguageIdAsync()
            : await db.Languages.Where(l => l.Code == languageCode && l.IsActive).Select(l => l.Id).FirstOrDefaultAsync();

        if (languageId == Guid.Empty)
        {
            languageId = await languageService.GetDefaultLanguageIdAsync();
        }

        var dtoById = menu.Items.ToDictionary(i => i.Id, i => new MenuItemPublicDto
        {
            Id = i.Id,
            Title = i.Translations.Where(t => t.LanguageId == languageId).Select(t => t.Title).FirstOrDefault() ?? string.Empty,
            Url = i.Url,
            ImgUrl = i.ImgUrl,
            Extra = i.Extra,
            Order = i.Order
        });

        var roots = new List<MenuItemPublicDto>();
        foreach (var item in menu.Items.OrderBy(i => i.Order))
        {
            var dto = dtoById[item.Id];
            if (item.ParentId is { } parentId && dtoById.TryGetValue(parentId, out var parentDto))
            {
                parentDto.Children.Add(dto);
            }
            else
            {
                roots.Add(dto);
            }
        }

        return new MenuPublicDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Description = menu.Description,
            Location = menu.Location,
            Items = roots
        };
    }

    public async Task<ServiceResult> DeleteAsync(Guid menuId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var hasItems = await db.MenuItems.AnyAsync(i => i.MenuId == menuId);
        if (hasItems)
        {
            return ServiceResult.Fail("این منو آیتم دارد؛ ابتدا آیتم‌های آن را حذف کنید");
        }

        var menu = await db.Menus.FindAsync(menuId);
        if (menu is null)
        {
            return ServiceResult.Fail("منو یافت نشد");
        }

        menu.IsDeleted = true;
        menu.DeletedAt = DateTime.UtcNow;
        menu.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
