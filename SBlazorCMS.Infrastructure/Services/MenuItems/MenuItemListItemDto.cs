namespace SBlazorCMS.Infrastructure.Services.MenuItems;

public class MenuItemListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? ParentTitle { get; set; }
}
