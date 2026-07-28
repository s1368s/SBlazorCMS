using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Menus;

public interface IMenuService
{
    Task<List<MenuListItemDto>> GetListAsync();
    Task<string?> GetNameAsync(Guid menuId);
    Task<MenuEditDto?> GetForEditAsync(Guid menuId);
    Task<ServiceResult> SaveAsync(MenuSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid menuId, Guid? currentUserId);
}
