namespace SBlazorCMS.Domain;

public class TagTranslation : BaseEntity<Guid>
{
    public Guid TagId { get; set; }
    public Tag? Tag { get; set; }
    public Guid LanguageId { get; set; }
    public Language? Language { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
