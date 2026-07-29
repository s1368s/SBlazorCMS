using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Media;

public interface IMediaService
{
    Task<List<MediaListItemDto>> GetListAsync();
    Task<ServiceResult> UploadAsync(MediaUploadRequest request);
    Task<ServiceResult> DeleteAsync(Guid mediaId, Guid? currentUserId);
}
