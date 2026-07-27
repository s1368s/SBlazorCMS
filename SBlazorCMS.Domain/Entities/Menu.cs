namespace SBlazorCMS.Domain;

public class Menu : AuditableEntity<Guid>
{
    public required string Name { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImgUrl { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid? ParentId { get; set; }
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();
}