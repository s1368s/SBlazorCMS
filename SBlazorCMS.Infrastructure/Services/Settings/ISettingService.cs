using SBlazorCMS.Contracts.Settings;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Settings;

public interface ISettingService
{
    Task<List<SettingListItemDto>> GetListAsync();
    Task<SettingEditDto?> GetForEditAsync(Guid settingId);
    Task<ServiceResult> SaveAsync(SettingSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid settingId, Guid? currentUserId);
    Task<List<SettingPublicDto>> GetByKeysAsync(List<string> keys);
}
