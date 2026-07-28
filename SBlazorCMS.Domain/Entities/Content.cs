namespace SBlazorCMS.Domain;

public class Content : AuditableEntity<System.Guid>
{
    public DateTime? PublishDate { get; set; }
    public ContentStatus Status { get; set; }
    public Guid AuthorId { get; set; }

    public string BigImg { get; set; } = string.Empty;
    public string SmallImg { get; set; } = string.Empty;
    public int OrderValue { get; set; }
    public Guid? SkinId { get; set; }
    public Skin? Skin { get; set; }

    public ICollection<ContentTranslation> Translations { get; set; } = new List<ContentTranslation>();
}
