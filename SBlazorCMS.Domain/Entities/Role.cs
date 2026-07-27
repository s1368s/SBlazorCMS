namespace SBlazorCMS.Domain;

public class Role : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}