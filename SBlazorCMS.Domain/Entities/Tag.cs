namespace SBlazorCMS.Domain;

public class Tag : AuditableEntity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}