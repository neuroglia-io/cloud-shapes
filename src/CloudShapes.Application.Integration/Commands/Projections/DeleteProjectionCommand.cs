namespace CloudShapes.Application.Commands.Projections;

/// <summary>
/// Represents the command used to delete a projection
/// </summary>
/// <param name="type">The name of the type of projection to delete</param>
public class DeleteProjectionCommand(string type, string id)
    : Command
{

    /// <summary>
    /// Gets the name of the type of projection to delete
    /// </summary>
    public virtual string Type { get; } = type;

    /// <summary>
    /// Gets the id of the projection to delete
    /// </summary>
    public virtual string Id { get; } = id;

}