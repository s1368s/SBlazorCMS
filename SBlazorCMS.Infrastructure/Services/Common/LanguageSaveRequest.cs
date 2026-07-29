namespace SBlazorCMS.Infrastructure.Services.Common;

public class LanguageSaveRequest
{
    public Guid? LanguageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CurrentUserId { get; set; }
}
