namespace SBlazorCMS.Domain;

public class Content : AuditableEntity<System.Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string PreTitle { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;

    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public DateTime? PublishDate { get; set; }
    public ContentStatus Status { get; set; }
    public Guid AuthorId { get; set; }

    public string BigImg { get; set; } = string.Empty;
    public string SmallImg { get; set; } = string.Empty;
    public int OrderValue { get; set; }
    public Guid? SkinId { get; set; }
    public Skin? Skin { get; set; }
}