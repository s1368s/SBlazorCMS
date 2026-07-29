using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.Common;
using MediaEntity = SBlazorCMS.Domain.Media;

namespace SBlazorCMS.Infrastructure.Services.Media;

public class MediaService(IDbContextFactory<ApplicationDbContext> dbFactory, IUploadPathProvider pathProvider) : IMediaService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public async Task<List<MediaListItemDto>> GetListAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Media
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MediaListItemDto
            {
                Id = m.Id,
                OriginalName = m.OriginalName,
                Address = m.Address,
                Extension = m.Extension,
                MimeType = m.MimeType,
                Size = m.Size,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ServiceResult> UploadAsync(MediaUploadRequest request)
    {
        var extension = Path.GetExtension(request.OriginalFileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return ServiceResult.Fail("فرمت فایل مجاز نیست. فرمت‌های مجاز: jpg، jpeg، png، gif، webp، svg");
        }

        if (request.Size > MaxFileSizeBytes)
        {
            return ServiceResult.Fail("حجم فایل نباید بیشتر از ۱۰ مگابایت باشد");
        }

        var uploadsFolder = Path.Combine(pathProvider.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var physicalPath = Path.Combine(uploadsFolder, storedFileName);

        await using (var fileStream = new FileStream(physicalPath, FileMode.Create))
        {
            await request.Content.CopyToAsync(fileStream);
        }

        await using var db = await dbFactory.CreateDbContextAsync();
        var media = new MediaEntity
        {
            Id = Guid.NewGuid(),
            FileName = storedFileName,
            OriginalName = request.OriginalFileName,
            Extension = extension,
            MimeType = request.ContentType,
            Size = request.Size,
            Address = $"/uploads/{storedFileName}",
            Path = physicalPath,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CurrentUserId
        };

        db.Media.Add(media);
        await db.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(Guid mediaId, Guid? currentUserId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var media = await db.Media.FindAsync(mediaId);
        if (media is null)
        {
            return ServiceResult.Fail("رسانه یافت نشد");
        }

        media.IsDeleted = true;
        media.DeletedAt = DateTime.UtcNow;
        media.DeletedBy = currentUserId;

        await db.SaveChangesAsync();

        if (File.Exists(media.Path))
        {
            File.Delete(media.Path);
        }

        return ServiceResult.Ok();
    }
}
