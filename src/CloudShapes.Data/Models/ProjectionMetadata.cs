namespace CloudShapes.Data.Models;

/// <summary>
/// Represents an object that holds information about a specific projection
/// </summary>
public record ProjectionMetadata
{

    /// <summary>
    /// Gets the name of the BSON property used to store a projection's metadata
    /// </summary>
    public const string PropertyName = "_metadata";

    /// <summary>
    /// Gets/sets the date and time the projection was created at
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets/sets the date and time the projection was last modified at
    /// </summary>
    public virtual DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets/sets the projection's version number
    /// </summary>
    public virtual int Version { get; set; } = 1;

    /// <summary>
    /// Updates the <see cref="ProjectionMetadata"/>
    /// </summary>
    public virtual void Update()
    {
        LastModified = DateTimeOffset.Now;
        Version++;
    }

}
