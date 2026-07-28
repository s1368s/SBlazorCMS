namespace SBlazorCMS.Infrastructure.Services.MenuItems;

public class MenuItemEditDto
{
    public Guid? ParentId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string ImgUrl { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;
    public int Order { get; set; }
    public Dictionary<Guid, MenuItemTranslationInput> Translations { get; set; } = new();
}
