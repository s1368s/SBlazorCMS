using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Categories;

public interface ICategoryService
{
    Task<List<SkinOptionDto>> GetSkinOptionsAsync();
    Task<List<CategoryListItemDto>> GetListAsync();
    Task<List<CategoryOptionDto>> GetParentOptionsAsync(Guid? excludeCategoryId);
    Task<CategoryEditDto?> GetForEditAsync(Guid categoryId);
    Task<ServiceResult> SaveAsync(CategorySaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid categoryId, Guid? currentUserId);
}
