using Microsoft.EntityFrameworkCore;
using SBlazorCMS.Contracts.Common;
using SBlazorCMS.Domain;
using SBlazorCMS.Infrastructure.Persistence;

namespace SBlazorCMS.Infrastructure.Services.ActivityLogs;

public class ActivityLogService(IDbContextFactory<ApplicationDbContext> dbFactory) : IActivityLogService
{
    public async Task LogAsync(Guid? userId, string action, string entityName, string? entityId = null, string? details = null, string? ipAddress = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        db.ActivityLogs.Add(new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }

    public async Task<PagedResult<ActivityLogListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? action = null,
        string? entityName = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

        await using var db = await dbFactory.CreateDbContextAsync();

        var query = db.ActivityLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(a => a.EntityName == entityName);
        }

        if (fromDate is { } from)
        {
            query = query.Where(a => a.CreatedAt >= from);
        }

        if (toDate is { } to)
        {
            query = query.Where(a => a.CreatedAt <= to);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.UserId,
                a.Action,
                a.EntityName,
                a.EntityId,
                a.Details,
                a.IpAddress,
                a.CreatedAt
            })
            .ToListAsync();

        var userIds = items.Where(a => a.UserId.HasValue).Select(a => a.UserId!.Value).Distinct().ToList();
        var userNames = await db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.UserName })
            .ToDictionaryAsync(u => u.Id, u => u.UserName);

        var dtos = items.Select(a => new ActivityLogListItemDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserName = a.UserId.HasValue && userNames.TryGetValue(a.UserId.Value, out var name) ? name : null,
            Action = a.Action,
            EntityName = a.EntityName,
            EntityId = a.EntityId,
            Details = a.Details,
            IpAddress = a.IpAddress,
            CreatedAt = a.CreatedAt
        }).ToList();

        return new PagedResult<ActivityLogListItemDto>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<List<string>> GetDistinctActionsAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ActivityLogs.Select(a => a.Action).Distinct().OrderBy(a => a).ToListAsync();
    }

    public async Task<List<string>> GetDistinctEntityNamesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ActivityLogs.Select(a => a.EntityName).Distinct().OrderBy(a => a).ToListAsync();
    }
}
