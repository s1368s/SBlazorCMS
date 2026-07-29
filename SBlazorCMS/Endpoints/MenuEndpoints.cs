using SBlazorCMS.Infrastructure.Services.Menus;

namespace SBlazorCMS.Endpoints;

public static class MenuEndpoints
{
    public static void MapMenuEndpoints(this WebApplication app)
    {
        app.MapGet("/api/menus/{name}", HandleGetByNameAsync);
    }

    private static async Task<IResult> HandleGetByNameAsync(string name, string? lang, IMenuService menuService)
    {
        var menu = await menuService.GetPublicByNameAsync(name, lang);
        return menu is null ? Results.NotFound() : Results.Ok(menu);
    }
}
