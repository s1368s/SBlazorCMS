using SBlazorCMS.Domain;

namespace SBlazorCMS.Infrastructure.Services.Contents;

public class ContentEditDto
{
    public ContentStatus Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public string BigImg { get; set; } = string.Empty;
    public string SmallImg { get; set; } = string.Empty;
    public int OrderValue { get; set; }
    public Guid? SkinId { get; set; }
    public List<Guid> CategoryIds { get; set; } = [];
    public List<Guid> TagIds { get; set; } = [];
    public Dictionary<Guid, ContentTranslationInput> Translations { get; set; } = new();
}
