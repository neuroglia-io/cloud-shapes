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
    /// Gets/sets a list containing the properties on which the index is defined
    /// </summary>
    public virtual EquatableList<string> Properties { get; set; } = null!;

    /// <summary>
    /// Gets/sets a boolean that defines whether or not the index is unique
    /// </summary>
    public virtual bool Unique { get; set; }

    /// <summary>
    /// Gets/sets a boolean that defines whether or not the index is sorted in a descending fashion
    /// </summary>
    public virtual bool Descending { get; set; }

    /// <summary>
    /// Gets/sets a boolean that defines whether or not the index supports full text searches
    /// </summary>
    public virtual bool Text { get; set; }

}
