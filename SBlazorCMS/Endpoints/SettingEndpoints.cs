using SBlazorCMS.Contracts.Settings;
using SBlazorCMS.Infrastructure.Services.Settings;

namespace SBlazorCMS.Endpoints;

public static class SettingEndpoints
{
    public static void MapSettingEndpoints(this WebApplication app)
    {
        app.MapPost("/api/settings/by-keys", HandleGetByKeysAsync)
            .WithName("GetSettingsByKeys")
            .WithTags("Settings")
            .WithSummary("دریافت لیستی از تنظیمات بر اساس کلیدها")
            .WithDescription("برای هر کلید موجود در لیست ارسالی، مقدار تنظیمات مربوطه را برمی‌گرداند. کلیدهایی که تنظیمی برایشان ثبت نشده در پاسخ حذف می‌شوند.")
            .Produces<List<SettingPublicDto>>();
    }

    private static async Task<IResult> HandleGetByKeysAsync(SettingKeysRequest request, ISettingService settingService)
    {
        var settings = await settingService.GetByKeysAsync(request.Keys);
        return Results.Ok(settings);
    }
}
