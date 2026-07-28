namespace SBlazorCMS.Infrastructure.Services.Common;

public class ServiceResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public static ServiceResult Ok() => new() { Success = true };
    public static ServiceResult Fail(string message) => new() { Success = false, ErrorMessage = message };
}
