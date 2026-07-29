namespace SBlazorCMS.Infrastructure.Services.Settings;

public class SettingSaveRequest
{
    public Guid? SettingId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Guid? CurrentUserId { get; set; }
}
