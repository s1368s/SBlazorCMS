namespace SBlazorCMS.Infrastructure.Services.Categories;

public class CategorySaveRequest
{
    public Guid? CategoryId { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? SkinId { get; set; }
    public int OrderValue { get; set; }
    public int ShowCount { get; set; }
    public Dictionary<Guid, CategoryTranslationInput> Translations { get; set; } = new();
    public Guid? CurrentUserId { get; set; }
}
