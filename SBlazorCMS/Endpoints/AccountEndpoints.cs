using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Authorization;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;
using SBlazorCMS.Infrastructure.Services.ActivityLogs;

namespace SBlazorCMS.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var loginBuilder = app.MapPost("/account/login", (HttpContext http, IAntiforgery antiforgery,
                IDbContextFactory<ApplicationDbContext> dbFactory, IPasswordHasher<User> hasher, IActivityLogService activityLogService)
            => HandleLoginAsync(http, antiforgery, dbFactory, hasher, activityLogService));
        loginBuilder.DisableAntiforgery();
        loginBuilder.ExcludeFromDescription();

        var logoutBuilder = app.MapPost("/account/logout", (HttpContext http, IActivityLogService activityLogService)
            => HandleLogoutAsync(http, activityLogService));
        logoutBuilder.DisableAntiforgery();
        logoutBuilder.ExcludeFromDescription();
    }

    private static async Task<IResult> HandleLoginAsync(
        HttpContext http,
        IAntiforgery antiforgery,
        IDbContextFactory<ApplicationDbContext> dbFactory,
        IPasswordHasher<User> hasher,
        IActivityLogService activityLogService)
    {
        if (!await antiforgery.IsRequestValidAsync(http))
        {
            return Results.Redirect("/login?error=1");
        }

        var form = await http.Request.ReadFormAsync();
        var userName = form["userName"].ToString().Trim();
        var password = form["password"].ToString();
        var returnUrl = form["returnUrl"].ToString();
        var ipAddress = http.Connection.RemoteIpAddress?.ToString();

        await using var db = await dbFactory.CreateDbContextAsync();

        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user is null || !user.IsActive)
        {
            await activityLogService.LogAsync(user?.Id, "Login Failed", "User", null, $"نام کاربری: {userName}", ipAddress);
            return Results.Redirect("/login?error=1");
        }

        var verification = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            await activityLogService.LogAsync(user.Id, "Login Failed", "User", user.Id.ToString(), $"نام کاربری: {userName}", ipAddress);
            return Results.Redirect("/login?error=1");
        }

        var roleIds = await db.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToListAsync();
        var roleNames = await db.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToListAsync();
        var permissionIds = await db.RolePermissions.Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.PermissionId).Distinct().ToListAsync();
        var permissionCodes = await db.Permissions.Where(p => permissionIds.Contains(p.Id))
            .Select(p => p.Code).ToListAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };
        claims.AddRange(roleNames.Select(name => new Claim(ClaimTypes.Role, name)));
        claims.AddRange(permissionCodes.Select(code => new Claim(PermissionClaimTypes.Permission, code)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = true
        });

        await activityLogService.LogAsync(user.Id, "Login", "User", user.Id.ToString(), $"ورود کاربر: {user.UserName}", ipAddress);

        return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
    }

    private static async Task<IResult> HandleLogoutAsync(HttpContext http, IActivityLogService activityLogService)
    {
        var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            await activityLogService.LogAsync(userId, "Logout", "User", userId.ToString(), null, http.Connection.RemoteIpAddress?.ToString());
        }

        await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Redirect("/login");
    }
}
