using CloudShapes.Application.Commands.ProjectionTypes;

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
    /// Gets/sets the command used to create a new projection type
    /// </summary>
    public CreateProjectionTypeCommand Command { get; set; } = new();

    /// <summary>
    /// Gets/sets a list containing the errors that have occurred during the validation or the creation of the projection type
    /// </summary>
    public EquatableList<string> Errors { get; set; } = [];

}
