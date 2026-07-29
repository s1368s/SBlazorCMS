namespace SBlazorCMS.Infrastructure.Services.Categories;

public class CategoryListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? ParentTitle { get; set; }
    public int OrderValue { get; set; }
    public int ShowCount { get; set; }
}
