namespace CloudShapes.Data.Models;

/// <summary>
/// Represents an object that holds information about a specific document
/// </summary>
public record DocumentMetadata
{

    /// <summary>
    /// Gets the name of the BSON property used to store a document's metadata
    /// </summary>
    public const string PropertyName = "_metadata";

    /// <summary>
    /// Gets/sets the date and time the document was created at
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets/sets the date and time the document was last modified at
    /// </summary>
    public virtual DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets/sets the document's version number
    /// </summary>
    public virtual int Version { get; set; } = 1;

    /// <summary>
    /// Updates the <see cref="DocumentMetadata"/>
    /// </summary>
    public virtual void Update()
    {
        LastModified = DateTimeOffset.Now;
        Version++;
    }

}
