namespace SBlazorCMS.Infrastructure.Services.Media;

public class MediaUploadRequest
{
    public required Stream Content { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public Guid? CurrentUserId { get; set; }
}
