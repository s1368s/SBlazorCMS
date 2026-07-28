namespace SBlazorCMS.Domain;

public class MenuItem : AuditableEntity<Guid>
{
    public Guid MenuId { get; set; }
    public Menu? Menu { get; set; }

    public string Url { get; set; } = string.Empty;
    public string ImgUrl { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;
    public int Order { get; set; }

    public Guid? ParentId { get; set; }
    public MenuItem? Parent { get; set; }
    public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();

    public ICollection<MenuItemTranslation> Translations { get; set; } = new List<MenuItemTranslation>();
}
