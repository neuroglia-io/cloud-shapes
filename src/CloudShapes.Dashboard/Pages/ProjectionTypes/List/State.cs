namespace CloudShapes.Dashboard.Pages.ProjectionTypes.List;

/// <summary>
/// Represents the projection type list view state
/// </summary>
public record ProjectionTypeListState
{

    /// <summary>
    /// Gets/sets a boolean value that indicates whether data is currently being gathered
    /// </summary>
    public bool Loading { get; set; } = true;

    /// <summary>
    /// Gets/sets the current page of <see cref="ProjectionType"/>s to list
    /// </summary>
    public PagedResult<ProjectionType>? ProjectionTypes { get; set; }

}
