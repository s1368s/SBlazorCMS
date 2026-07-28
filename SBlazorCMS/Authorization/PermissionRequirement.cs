using Microsoft.AspNetCore.Authorization;

namespace SBlazorCMS.Authorization;

public class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
{
    public string PermissionCode { get; } = permissionCode;
}
