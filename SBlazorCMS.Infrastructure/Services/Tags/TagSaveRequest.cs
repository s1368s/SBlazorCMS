namespace SBlazorCMS.Infrastructure.Services.Tags;

public class TagSaveRequest
{
    public Guid? TagId { get; set; }
    public Dictionary<Guid, TagTranslationInput> Translations { get; set; } = new();
    public Guid? CurrentUserId { get; set; }
}
