namespace CloudShapes.Application.Commands.Projections;

/// <summary>
/// Represents the command used to create a new projection
/// </summary>
/// <param name="type">The name of the type of projection to create</param>
/// <param name="id">The id of the projection to create</param>
/// <param name="state">The initial state of the projection to create</param>
public class CreateProjectionCommand(string type, string id, IDictionary<string, object> state)
    : Command<object>
{

    /// <summary>
    /// Gets the name of the type of projection to create
    /// </summary>
    public string Type { get; } = type;

    /// <summary>
    /// Gets/sets the id of the projection to create
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Gets the initial state of the projection to create
    /// </summary>
    public IDictionary<string, object> State { get; } = state;

}
