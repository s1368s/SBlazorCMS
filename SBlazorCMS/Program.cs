using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using SBlazorCMS;
using SBlazorCMS.Authorization;
using SBlazorCMS.Components;
using SBlazorCMS.Domain;
using SBlazorCMS.Endpoints;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.Categories;
using SBlazorCMS.Infrastructure.Services.Comments;
using SBlazorCMS.Infrastructure.Services.Common;
using SBlazorCMS.Infrastructure.Services.Contents;
using SBlazorCMS.Infrastructure.Services.Media;
using SBlazorCMS.Infrastructure.Services.MenuItems;
using SBlazorCMS.Infrastructure.Services.Menus;
using SBlazorCMS.Infrastructure.Services.Roles;
using SBlazorCMS.Infrastructure.Services.Settings;
using SBlazorCMS.Infrastructure.Services.Skins;
using SBlazorCMS.Infrastructure.Services.Tags;
using SBlazorCMS.Infrastructure.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 15 * 1024 * 1024;
    });

builder.Services.AddMudServices(config =>
{
    config.PopoverOptions.CheckForPopoverProvider = false;
});

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ISkinService, SkinService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddSingleton<IUploadPathProvider, WebRootUploadPathProvider>();

builder.Services.AddAntiforgery();

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapAccountEndpoints();
app.MapMenuEndpoints();

app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
