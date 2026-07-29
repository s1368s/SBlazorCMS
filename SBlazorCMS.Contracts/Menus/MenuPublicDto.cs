namespace SBlazorCMS.Contracts.Menus;

public class MenuPublicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<MenuItemPublicDto> Items { get; set; } = new();
}
