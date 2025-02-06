namespace CloudShapes.Application.Commands.ProjectionTypes;

/// <summary>
/// Represents the command used to delete a projection type
/// </summary>
/// <param name="name">The name of the projection type to delete</param>
public class DeleteProjectionTypeCommand(string name)
    : Command
{

    /// <summary>
    /// Gets the name of the projection type to delete
    /// </summary>
    public virtual string Name { get; } = name;

}