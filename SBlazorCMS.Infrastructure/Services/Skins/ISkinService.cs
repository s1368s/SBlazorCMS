using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Skins;

public interface ISkinService
{
    Task<List<SkinListItemDto>> GetListAsync();
    Task<SkinEditDto?> GetForEditAsync(Guid skinId);
    Task<ServiceResult> SaveAsync(SkinSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid skinId, Guid? currentUserId);
}
