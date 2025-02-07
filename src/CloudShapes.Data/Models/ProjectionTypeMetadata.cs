namespace CloudShapes.Data.Models;

/// <summary>
/// Represents the metadata of a projection type document
/// </summary>
public record ProjectionTypeMetadata
    : DocumentMetadata
{

    /// <summary>
    /// Gets/sets the total count of projections of this type
    /// </summary>
    public virtual long ProjectionCount { get; set; }

}
