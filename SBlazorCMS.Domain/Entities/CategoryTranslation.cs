namespace SBlazorCMS.Domain;

public class CategoryTranslation : BaseEntity<Guid>
{
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public Guid LanguageId { get; set; }
    public Language? Language { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
