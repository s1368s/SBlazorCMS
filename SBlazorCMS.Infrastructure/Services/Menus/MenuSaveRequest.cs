namespace SBlazorCMS.Infrastructure.Services.Menus;

public class MenuSaveRequest
{
    public Guid? MenuId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Guid? CurrentUserId { get; set; }
}
