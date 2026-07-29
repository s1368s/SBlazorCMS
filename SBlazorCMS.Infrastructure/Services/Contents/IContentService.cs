using SBlazorCMS.Contracts.Common;
using SBlazorCMS.Contracts.Contents;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Contents;

public interface IContentService
{
    Task<List<ContentListItemDto>> GetListAsync();
    Task<ContentEditDto?> GetForEditAsync(Guid contentId);
    Task<ServiceResult> SaveAsync(ContentSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid contentId, Guid? currentUserId);
    Task<PagedResult<ContentListItemPublicDto>> GetPublicByCategoryCodeAsync(string categoryCode, int page, int pageSize, string? languageCode);
    Task<ContentDetailPublicDto?> GetPublicByIdAsync(Guid contentId, string? languageCode);
}
