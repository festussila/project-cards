using Microsoft.EntityFrameworkCore;

namespace Core.Management.Common.Projections;

public class PaginatedList<T>
{
    private PaginatedList(List<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));
    }

    public List<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; set; }
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;

    public static async Task<PaginatedList<T>> GetAsync(IQueryable<T> query, int page, int pageSize)
    {
        int totalCount = await query.CountAsync().ConfigureAwait(false);
        List<T> items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

        return new(items, page, pageSize, totalCount);
    }
}