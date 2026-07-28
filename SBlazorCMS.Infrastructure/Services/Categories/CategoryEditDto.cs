namespace SBlazorCMS.Infrastructure.Services.Categories;

public class CategoryEditDto
{
    public Guid? ParentId { get; set; }
    public Guid? SkinId { get; set; }
    public int OrderValue { get; set; }
    public int ShowCount { get; set; }
    public Dictionary<Guid, CategoryTranslationInput> Translations { get; set; } = new();
}
