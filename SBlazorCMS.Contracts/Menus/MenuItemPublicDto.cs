namespace SBlazorCMS.Contracts.Menus;

public class MenuItemPublicDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImgUrl { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<MenuItemPublicDto> Children { get; set; } = new();
}
