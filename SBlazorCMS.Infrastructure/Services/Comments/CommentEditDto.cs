namespace SBlazorCMS.Infrastructure.Services.Comments;

public class CommentEditDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
}
