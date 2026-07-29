using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.ActivityLogs;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Comments;

public class CommentService(IDbContextFactory<ApplicationDbContext> dbFactory, ILanguageService languageService, IActivityLogService activityLogService) : ICommentService
{
    public async Task<List<CommentListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var defaultLangId = await languageService.GetDefaultLanguageIdAsync();

        return await db.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentListItemDto
            {
                Id = c.Id,
                ContentTitle = db.ContentTranslations
                    .Where(t => t.ContentId == c.ContentId && t.LanguageId == defaultLangId)
                    .Select(t => t.Title)
                    .FirstOrDefault() ?? "(بدون عنوان)",
                Name = c.Name,
                Email = c.Email,
                Text = c.Text,
                IsApproved = c.IsApproved,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<CommentEditDto?> GetForEditAsync(Guid commentId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var comment = await db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment is null)
        {
            return null;
        }

        return new CommentEditDto
        {
            Name = comment.Name,
            Email = comment.Email,
            Text = comment.Text,
            IsApproved = comment.IsApproved
        };
    }

    public async Task<ServiceResult> UpdateAsync(CommentUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return ServiceResult.Fail("متن دیدگاه الزامی است");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        var comment = await db.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId);
        if (comment is null)
        {
            return ServiceResult.Fail("دیدگاه یافت نشد");
        }

        comment.Name = request.Name;
        comment.Email = request.Email;
        comment.Text = request.Text;
        comment.IsApproved = request.IsApproved;
        comment.UpdatedAt = DateTime.UtcNow;
        comment.UpdatedBy = request.CurrentUserId;

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(request.CurrentUserId, "Update", "Comment", comment.Id.ToString(), comment.Name);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> SetApprovalAsync(Guid commentId, bool isApproved, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var comment = await db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        if (comment is null)
        {
            return ServiceResult.Fail("دیدگاه یافت نشد");
        }

        comment.IsApproved = isApproved;
        comment.UpdatedAt = DateTime.UtcNow;
        comment.UpdatedBy = currentUserId;

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(currentUserId, isApproved ? "Approve" : "Unapprove", "Comment", commentId.ToString(), comment.Name);
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(Guid commentId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var comment = await db.Comments.FindAsync(commentId);
        if (comment is null)
        {
            return ServiceResult.Fail("دیدگاه یافت نشد");
        }

        comment.IsDeleted = true;
        comment.DeletedAt = DateTime.UtcNow;
        comment.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        await activityLogService.LogAsync(currentUserId, "Delete", "Comment", commentId.ToString(), comment.Name);
        return ServiceResult.Ok();
    }
}
