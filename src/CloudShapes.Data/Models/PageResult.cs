namespace CloudShapes.Data.Models;

/// <summary>
/// Represents a paged result containing a subset of items from a larger dataset
/// </summary>
/// <param name="Items">The items in the current page</param>
/// <param name="TotalCount">The total number of items available in the dataset</param>
/// <param name="PageSize">The number of items returned in this collection</param>
/// <param name="CurrentPage">The current page number</param>
public record PagedResult<T>(IEnumerable<T> Items, long TotalCount, int? PageSize, int? CurrentPage)
{

    /// <summary>
    /// Gets the total number of pages available.
    /// </summary>
    [JsonIgnore]
    public virtual int? TotalPages => PageSize.HasValue ? PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize.Value) : 0 : null;

}
