using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Services.Contents;

public class ContentListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContentStatus Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public int OrderValue { get; set; }
}
