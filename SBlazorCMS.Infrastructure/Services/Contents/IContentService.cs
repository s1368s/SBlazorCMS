using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Contents;

public interface IContentService
{
    Task<List<ContentListItemDto>> GetListAsync();
    Task<ContentEditDto?> GetForEditAsync(Guid contentId);
    Task<ServiceResult> SaveAsync(ContentSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid contentId, Guid? currentUserId);
}
