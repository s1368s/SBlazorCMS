using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Comments;

public interface ICommentService
{
    Task<List<CommentListItemDto>> GetListAsync();
    Task<CommentEditDto?> GetForEditAsync(Guid commentId);
    Task<ServiceResult> UpdateAsync(CommentUpdateRequest request);
    Task<ServiceResult> SetApprovalAsync(Guid commentId, bool isApproved, Guid? currentUserId);
    Task<ServiceResult> DeleteAsync(Guid commentId, Guid? currentUserId);
}
