namespace SBlazorCMS.Infrastructure.Services.Media;

public class MediaListItemDto
{
    public Guid Id { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
}
