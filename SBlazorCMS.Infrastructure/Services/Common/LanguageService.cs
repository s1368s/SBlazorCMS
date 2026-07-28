using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Infrastructure.Persistence;

namespace SBlazorCMS.Infrastructure.Services.Common;

public class LanguageService(IDbContextFactory<ApplicationDbContext> dbFactory) : ILanguageService
{
    public async Task<List<LanguageDto>> GetActiveLanguagesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Languages
            .Where(l => l.IsActive)
            .OrderByDescending(l => l.IsDefault)
            .Select(l => new LanguageDto(l.Id, l.Code, l.Name, l.IsDefault))
            .ToListAsync();
    }

    public async Task<Guid> GetDefaultLanguageIdAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Languages.Where(l => l.IsDefault).Select(l => l.Id).FirstAsync();
    }
}
