namespace SBlazorCMS.Infrastructure.Services.Tags;

public class TagEditDto
{
    public Dictionary<Guid, TagTranslationInput> Translations { get; set; } = new();
}
