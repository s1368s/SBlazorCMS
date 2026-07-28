namespace SBlazorCMS.Domain;

public class ContentRevision : BaseEntity<Guid>
{
    public Guid ContentId { get; set; }
    public Content? Content { get; set; }
    public Guid LanguageId { get; set; }
    public Language? Language { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public ContentStatus Status { get; set; }

    public Guid EditedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
