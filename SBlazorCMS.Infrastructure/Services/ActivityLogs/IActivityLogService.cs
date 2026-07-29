using SBlazorCMS.Contracts.Common;

namespace SBlazorCMS.Infrastructure.Services.ActivityLogs;

public interface IActivityLogService
{
    Task LogAsync(Guid? userId, string action, string entityName, string? entityId = null, string? details = null, string? ipAddress = null);

    Task<PagedResult<ActivityLogListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? action = null,
        string? entityName = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    Task<List<string>> GetDistinctActionsAsync();
    Task<List<string>> GetDistinctEntityNamesAsync();
}
