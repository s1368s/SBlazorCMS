namespace SBlazorCMS.Infrastructure.Services.Common;

public interface ILanguageService
{
    Task<List<LanguageDto>> GetActiveLanguagesAsync();
    Task<Guid> GetDefaultLanguageIdAsync();
    Task<List<LanguageListItemDto>> GetListAsync();
    Task<LanguageEditDto?> GetForEditAsync(Guid languageId);
    Task<ServiceResult> SaveAsync(LanguageSaveRequest request);
    Task<ServiceResult> DeleteAsync(Guid languageId, Guid? currentUserId);
}
