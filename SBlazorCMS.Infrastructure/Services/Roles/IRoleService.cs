using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Roles;

public interface IRoleService
{
    Task<List<RoleListItemDto>> GetListAsync();
    Task<List<PermissionOptionDto>> GetPermissionOptionsAsync();
    Task<RoleEditDto?> GetForEditAsync(Guid roleId);
    Task<ServiceResult> SaveAsync(RoleSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid roleId, Guid? currentUserId);
}
