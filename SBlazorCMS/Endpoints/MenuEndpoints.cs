using SBlazorCMS.Contracts.Menus;
using SBlazorCMS.Infrastructure.Services.Menus;

namespace SBlazorCMS.Endpoints;

public static class MenuEndpoints
{
    public static void MapMenuEndpoints(this WebApplication app)
    {
        app.MapGet("/api/menus/{name}", HandleGetByNameAsync)
            .WithName("GetMenuByName")
            .WithTags("Menus")
            .WithSummary("دریافت یک منو به همراه آیتم‌هایش")
            .WithDescription("منو را با نام دقیق (یکتا) پیدا می‌کند و درخت آیتم‌ها را بر اساس زبان مشخص‌شده (پارامتر lang، در صورت نبود: زبان پیش‌فرض) برمی‌گرداند.")
            .Produces<MenuPublicDto>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleGetByNameAsync(string name, string? lang, IMenuService menuService)
    {
        var menu = await menuService.GetPublicByNameAsync(name, lang);
        return menu is null ? Results.NotFound() : Results.Ok(menu);
    }
}
