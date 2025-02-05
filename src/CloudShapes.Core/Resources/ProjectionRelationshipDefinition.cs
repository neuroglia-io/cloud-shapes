namespace CloudShapes.Core.Resources;

/// <summary>
/// Represents the definition of a projection's relationship
/// </summary>
public record ProjectionRelationshipDefinition
{

    /// <summary>
    /// Gets/sets the relationship type
    /// </summary>
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets the name of the related projection type
    /// </summary>
    public virtual string Target { get; set; } = null!;

    /// <summary>
    /// Gets/sets the relationship's foreign key
    /// </summary>
    public virtual string Key { get; set; } = null!;

}