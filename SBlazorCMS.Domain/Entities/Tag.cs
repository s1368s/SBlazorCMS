namespace SBlazorCMS.Domain;

public class Tag : AuditableEntity<Guid>
{
    public ICollection<TagTranslation> Translations { get; set; } = new List<TagTranslation>();
}
