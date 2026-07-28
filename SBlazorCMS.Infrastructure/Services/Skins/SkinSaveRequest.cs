namespace SBlazorCMS.Infrastructure.Services.Skins;

public class SkinSaveRequest
{
    public Guid? SkinId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? CurrentUserId { get; set; }
}
