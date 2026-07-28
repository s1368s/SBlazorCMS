namespace SBlazorCMS.Domain;

public class Category : AuditableEntity<Guid>
{
    public int OrderValue { get; set; }
    public int ShowCount { get; set; }

    public Guid? SkinId { get; set; }
    public Skin? Skin { get; set; }
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();

    public ICollection<CategoryTranslation> Translations { get; set; } = new List<CategoryTranslation>();
}
