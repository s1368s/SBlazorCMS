namespace SBlazorCMS.Domain;

public class Skin : AuditableEntity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
