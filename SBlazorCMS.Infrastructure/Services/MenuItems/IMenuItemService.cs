using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.MenuItems;

public interface IMenuItemService
{
    Task<List<MenuItemListItemDto>> GetListByMenuAsync(Guid menuId);
    Task<List<MenuItemOptionDto>> GetParentOptionsAsync(Guid menuId, Guid? excludeItemId);
    Task<MenuItemEditDto?> GetForEditAsync(Guid menuItemId);
    Task<ServiceResult> SaveAsync(MenuItemSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid menuItemId, Guid? currentUserId);
}
