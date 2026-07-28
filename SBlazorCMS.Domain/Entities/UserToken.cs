namespace SBlazorCMS.Domain;

public class UserToken : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public UserTokenType Type { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
}
