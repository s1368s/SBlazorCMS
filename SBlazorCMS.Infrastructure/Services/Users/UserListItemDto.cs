namespace SBlazorCMS.Infrastructure.Services.Users;

public class UserListItemDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string RoleNames { get; set; } = string.Empty;
}
