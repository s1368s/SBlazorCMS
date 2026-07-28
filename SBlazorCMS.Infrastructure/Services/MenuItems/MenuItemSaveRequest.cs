namespace SBlazorCMS.Infrastructure.Services.MenuItems;

public class MenuItemSaveRequest
{
    public Guid? MenuItemId { get; set; }
    public Guid MenuId { get; set; }
    public Guid? ParentId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string ImgUrl { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;
    public int Order { get; set; }
    public Dictionary<Guid, MenuItemTranslationInput> Translations { get; set; } = new();
    public Guid? CurrentUserId { get; set; }
}
