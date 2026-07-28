using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Tags;

public interface ITagService
{
    Task<List<TagListItemDto>> GetListAsync();
    Task<TagEditDto?> GetForEditAsync(Guid tagId);
    Task<ServiceResult> SaveAsync(TagSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid tagId, Guid? currentUserId);
}
