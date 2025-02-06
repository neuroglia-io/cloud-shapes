namespace CloudShapes.Data.Models;

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

    /// <summary>
    /// Gets/sets the path to the relationship's navigation property<para></para>
    /// It can be the same than <see cref="Key"/>, in which case it replaces the key property with the related projection's state
    /// </summary>
    public virtual string Path { get; set; } = null!;

    /// <summary>
    /// Gets a boolean indicating whether the path to the relationship's foreign key is aligned with the path to its navigation property
    /// </summary>
    [BsonIgnore]
    public bool IsForeignKeyPathAligned => Key.Equals(Path);

}