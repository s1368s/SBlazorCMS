namespace SBlazorCMS.Infrastructure.Services.Settings;

public class SettingListItemDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
