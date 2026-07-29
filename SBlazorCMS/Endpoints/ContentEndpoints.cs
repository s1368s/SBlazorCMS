using SBlazorCMS.Contracts.Common;
using SBlazorCMS.Contracts.Contents;
using SBlazorCMS.Infrastructure.Services.Contents;

namespace SBlazorCMS.Endpoints;

public static class ContentEndpoints
{
    public static void MapContentEndpoints(this WebApplication app)
    {
        app.MapGet("/api/contents/by-category/{categoryCode}", HandleGetByCategoryCodeAsync)
            .WithName("GetContentsByCategoryCode")
            .WithTags("Contents")
            .WithSummary("دریافت محتواهای یک دسته‌بندی به‌صورت صفحه‌بندی‌شده")
            .WithDescription("محتواهای منتشرشده‌ی دسته‌بندی مشخص‌شده با کد آن (Category.Code) را جدیدترین به قدیمی‌ترین (بر اساس تاریخ ایجاد) و صفحه‌بندی‌شده برمی‌گرداند. پارامترهای page (پیش‌فرض 1)، pageSize (پیش‌فرض 10، حداکثر 100) و lang (کد زبان، پیش‌فرض زبان پیش‌فرض سیستم) قابل استفاده‌اند.")
            .Produces<PagedResult<ContentListItemPublicDto>>();
    }

    private static async Task<IResult> HandleGetByCategoryCodeAsync(
        string categoryCode, int? page, int? pageSize, string? lang, IContentService contentService)
    {
        var result = await contentService.GetPublicByCategoryCodeAsync(categoryCode, page ?? 1, pageSize ?? 10, lang);
        return Results.Ok(result);
    }
}
