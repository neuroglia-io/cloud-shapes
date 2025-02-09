namespace CloudShapes.Dashboard.Pages.ProjectionTypes.Create;

/// <summary>
/// Represents the state of the create projection type view
/// </summary>
public record CreateProjectionTypeState
{

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the view is loading
    /// </summary>
    public bool Loading { get; set; }

    /// <summary>
    /// Gets/sets the projection type to create
    /// </summary>
    public ProjectionType ProjectionType { get; set; } = new();

}
