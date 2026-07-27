namespace SBlazorCMS.Domain;

public class Comment : AuditableEntity<System.Guid>
{
    public Guid ContentId { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty; 
    public bool IsApproved { get; set; }
}