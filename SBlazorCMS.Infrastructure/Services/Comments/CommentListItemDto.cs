namespace SBlazorCMS.Infrastructure.Services.Comments;

public class CommentListItemDto
{
    public Guid Id { get; set; }
    public string ContentTitle { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}
