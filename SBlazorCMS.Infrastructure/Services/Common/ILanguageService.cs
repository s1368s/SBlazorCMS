namespace SBlazorCMS.Infrastructure.Services.Common;

public interface ILanguageService
{
    Task<List<LanguageDto>> GetActiveLanguagesAsync();
    Task<Guid> GetDefaultLanguageIdAsync();
}
