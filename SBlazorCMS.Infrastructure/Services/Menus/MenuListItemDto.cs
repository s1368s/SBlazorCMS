namespace SBlazorCMS.Infrastructure.Services.Menus;

public class MenuListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}
