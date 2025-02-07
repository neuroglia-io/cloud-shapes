namespace CloudShapes.Dashboard.Pages.Projections.List;

/// <summary>
/// Represents the projection list view state
/// </summary>
public record ProjectionListState
{

    /// <summary>
    /// Gets/sets a boolean value that indicates whether data is currently being gathered
    /// </summary>
    public bool Loading { get; set; } = true;

    /// <summary>
    /// Gets/sets a list of all available <see cref="Data.Models.ProjectionType"/>s
    /// </summary>
    public EquatableList<ProjectionType> ProjectionTypes { get; set; } = [];

    /// <summary>
    /// Gets/sets the name of the type of projections to list
    /// </summary>
    public string? ProjectionTypeName { get; set; }

    /// <summary>
    /// Gets/sets the type of projections to list
    /// </summary>
    public ProjectionType? ProjectionType { get; set; }

    /// <summary>
    /// Gets/sets the options used to query projections
    /// </summary>
    public QueryOptions QueryOptions { get; set; } = new();

    /// <summary>
    /// Gets/sets the current page of projections to list, if any
    /// </summary>
    public PagedResult<IDictionary<string, object>>? Projections { get; set; }

}
