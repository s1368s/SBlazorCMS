namespace SBlazorCMS.Domain;

public class MenuItemTranslation : BaseEntity<Guid>
{
    public Guid MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
    public Guid LanguageId { get; set; }
    public Language? Language { get; set; }

    public string Title { get; set; } = string.Empty;
}
