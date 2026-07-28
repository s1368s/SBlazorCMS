using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Authorization;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;

namespace SBlazorCMS.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        var loginBuilder = app.MapPost("/account/login", (HttpContext http, IAntiforgery antiforgery,
                IDbContextFactory<ApplicationDbContext> dbFactory, IPasswordHasher<User> hasher)
            => HandleLoginAsync(http, antiforgery, dbFactory, hasher));
        loginBuilder.DisableAntiforgery();

        var logoutBuilder = app.MapPost("/account/logout", (Delegate)((HttpContext http) => HandleLogoutAsync(http)));
        logoutBuilder.DisableAntiforgery();
    }

    private static async Task<IResult> HandleLoginAsync(
        HttpContext http,
        IAntiforgery antiforgery,
        IDbContextFactory<ApplicationDbContext> dbFactory,
        IPasswordHasher<User> hasher)
    {
        if (!await antiforgery.IsRequestValidAsync(http))
        {
            return Results.Redirect("/login?error=1");
        }

        var form = await http.Request.ReadFormAsync();
        var userName = form["userName"].ToString().Trim();
        var password = form["password"].ToString();
        var returnUrl = form["returnUrl"].ToString();

        await using var db = await dbFactory.CreateDbContextAsync();

        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user is null || !user.IsActive)
        {
            return Results.Redirect("/login?error=1");
        }

        var verification = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
        {
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

        return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
    }

    private static async Task<IResult> HandleLogoutAsync(HttpContext http)
    {
        await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Redirect("/login");
    }
}
