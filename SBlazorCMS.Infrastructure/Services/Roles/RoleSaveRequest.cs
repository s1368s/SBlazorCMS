namespace SBlazorCMS.Infrastructure.Services.Roles;

public class RoleSaveRequest
{
    public Guid? RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<Guid> PermissionIds { get; set; } = [];
    public Guid? CurrentUserId { get; set; }
}
