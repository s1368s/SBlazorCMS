namespace SBlazorCMS.Domain;

public class ContentTranslation : BaseEntity<Guid>
{
    public Guid ContentId { get; set; }
    public Content? Content { get; set; }
    public Guid LanguageId { get; set; }
    public Language? Language { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string PreTitle { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;

    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
}
