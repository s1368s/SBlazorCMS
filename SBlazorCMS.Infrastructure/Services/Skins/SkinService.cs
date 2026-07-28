using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.Common;

namespace SBlazorCMS.Infrastructure.Services.Skins;

public class SkinService(IDbContextFactory<ApplicationDbContext> dbFactory) : ISkinService
{
    public async Task<List<SkinListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Skins
            .OrderBy(s => s.Title)
            .Select(s => new SkinListItemDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description
            })
            .ToListAsync();
    }

    public async Task<SkinEditDto?> GetForEditAsync(Guid skinId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var skin = await db.Skins.FirstOrDefaultAsync(s => s.Id == skinId);
        if (skin is null)
        {
            return null;
        }

        return new SkinEditDto
        {
            Title = skin.Title,
            Description = skin.Description
        };
    }

    public async Task<ServiceResult> SaveAsync(SkinSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return ServiceResult.Fail("عنوان قالب الزامی است");
        }

        await using var db = await dbFactory.CreateDbContextAsync();

        Skin skin;
        if (request.SkinId is null)
        {
            skin = new Skin
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CurrentUserId
            };
            db.Skins.Add(skin);
        }
        else
        {
            var existing = await db.Skins.FirstOrDefaultAsync(s => s.Id == request.SkinId);
            if (existing is null)
            {
                return ServiceResult.Fail("قالب یافت نشد");
            }

            skin = existing;
            skin.UpdatedAt = DateTime.UtcNow;
            skin.UpdatedBy = request.CurrentUserId;
        }

        skin.Title = request.Title;
        skin.Description = request.Description;

        await db.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(Guid skinId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var skin = await db.Skins.FindAsync(skinId);
        if (skin is null)
        {
            return ServiceResult.Fail("قالب یافت نشد");
        }

        skin.IsDeleted = true;
        skin.DeletedAt = DateTime.UtcNow;
        skin.DeletedBy = currentUserId;

        await db.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
