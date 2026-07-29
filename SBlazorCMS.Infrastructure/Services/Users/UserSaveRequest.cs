namespace SBlazorCMS.Infrastructure.Services.Users;

public class UserSaveRequest
{
    public Guid? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? Password { get; set; }
    public List<Guid> RoleIds { get; set; } = [];
    public Guid? CurrentUserId { get; set; }
}
