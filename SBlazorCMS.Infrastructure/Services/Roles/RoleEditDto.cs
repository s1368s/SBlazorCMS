namespace SBlazorCMS.Infrastructure.Services.Roles;

public class RoleEditDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<Guid> PermissionIds { get; set; } = [];
}
