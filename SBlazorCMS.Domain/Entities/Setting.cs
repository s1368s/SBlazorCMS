namespace SBlazorCMS.Domain;

public class Setting : AuditableEntity<Guid>
{

    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}