using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Users;

public interface IUserService
{
    Task<List<UserListItemDto>> GetListAsync();
    Task<List<RoleOptionDto>> GetRoleOptionsAsync();
    Task<UserEditDto?> GetForEditAsync(Guid userId);
    Task<ServiceResult> SaveAsync(UserSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid userId, Guid? currentUserId);
}
