namespace SBlazorCMS.Infrastructure.Services.Roles;

public class RoleListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int PermissionCount { get; set; }
    public int UserCount { get; set; }
}
