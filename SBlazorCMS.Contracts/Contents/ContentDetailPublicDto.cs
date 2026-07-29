namespace SBlazorCMS.Contracts.Contents;

public class ContentDetailPublicDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string PreTitle { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public string BigImg { get; set; } = string.Empty;
    public string SmallImg { get; set; } = string.Empty;
    public DateTime? PublishDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
