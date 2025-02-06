namespace CloudShapes.Data.Models;

/// <summary>
/// Represents a paged result containing a subset of items from a larger dataset
/// </summary>
/// <param name="items">The items in the current page</param>
/// <param name="totalCount">The total number of items available in the dataset</param>
/// <param name="pageSize">The number of items returned in this collection</param>
/// <param name="currentPage">The current page number</param>
public class PagedResult<T>(IEnumerable<T> items, long totalCount, int? pageSize, int? currentPage)
{

    /// <summary>
    /// Gets/sets the total number of items available in the dataset
    /// </summary>
    public virtual long TotalCount { get; set; } = totalCount;

    /// <summary>
    /// Gets/sets the number of items returned in this collection
    /// </summary>
    public virtual int? PageSize { get; } = pageSize;

    /// <summary>
    /// Gets/sets the current page number
    /// </summary>
    public virtual int? CurrentPage { get; } = currentPage;

    /// <summary>
    /// Gets the total number of pages available.
    /// </summary>
    public virtual int? TotalPages => PageSize.HasValue ? PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize.Value) : 0 : null;

    /// <summary>
    /// Gets/sets the items in the current page
    /// </summary>
    public virtual List<T> Items { get; set; } = items.ToList();

}
