namespace CloudShapes.Data.Models;

/// <summary>
/// Represents a JSON Patch operation
/// </summary>
public record JsonPatchOperation
{

    /// <summary>
    /// Gets/sets the path, if any, to copy from for the Move/Copy operation
    /// </summary>
    public virtual string? From { get; set; }

    /// <summary>
    /// Gets/sets the type of operation to perform
    /// </summary>
    [Required]
    public virtual string Op { get; set; } = null!;

    /// <summary>
    /// Gets/sets the path of the property to patch
    /// </summary>
    [Required]
    public virtual string Path { get; set; } = null!;

    /// <summary>
    /// Gets/sets the patched value
    /// </summary>
    public virtual JsonElement? Value { get; set; }

}
