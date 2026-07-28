namespace SBlazorCMS.Domain;

public class Menu : AuditableEntity<Guid>
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
}
