namespace CloudShapes.Application.Commands.ProjectionTypes;

/// <summary>
/// Represents the command used to create a new <see cref="ProjectionType"/>
/// </summary>
public class CreateProjectionTypeCommand
    : Command<ProjectionType>
{

    /// <summary>
    /// Gets/sets the projection type's name
    /// </summary>
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets the schema that defines, documents and validates the state of projections of this type
    /// </summary>
    public virtual JSchema Schema { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for creating new projections when specific CloudEvents occur
    /// </summary>
    public virtual ProjectionTriggerCollection Triggers { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the indexes, if any, of projections of this type
    /// </summary>
    public virtual IReadOnlyCollection<ProjectionIndexDefinition>? Indexes { get; set; }

    /// <summary>
    /// Gets/sets a list containing the relationships, if any, of projections of this type
    /// </summary>
    public virtual IReadOnlyCollection<ProjectionRelationshipDefinition>? Relationships { get; set; }

}
