namespace SBlazorCMS.Domain;

public class Media : AuditableEntity<System.Guid>
{
    public string FileName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty; 
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
    public string MimeType { get; set; } = string.Empty; 
    public string Path { get; set; } = string.Empty;
}