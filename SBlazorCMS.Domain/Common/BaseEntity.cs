namespace SBlazorCMS.Domain;

public abstract class BaseEntity<TKey> { 
    public TKey Id { get; set; } = default!; }