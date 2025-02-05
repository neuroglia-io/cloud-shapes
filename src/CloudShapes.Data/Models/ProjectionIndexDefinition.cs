namespace CloudShapes.Data.Models;

/// <summary>
/// Represents the definition of a projection's index
/// </summary>
public record ProjectionIndexDefinition
{

    /// <summary>
    /// Gets/sets the index's name
    /// </summary>
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the fields on which the index is defined
    /// </summary>
    public virtual EquatableList<string> Fields { get; set; } = null!;

    /// <summary>
    /// Gets/sets a boolean that defines whether or not the index is unique
    /// </summary>
    public virtual bool Unique { get; set; }

    /// <summary>
    /// Gets/sets a boolean that defines whether or not the index supports full text searches
    /// </summary>
    public virtual bool Text { get; set; }

}
