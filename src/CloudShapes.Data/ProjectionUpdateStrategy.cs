namespace CloudShapes.Data;

/// <summary>
/// Enumerates all supported projection update strategies
/// </summary>
public static class ProjectionUpdateStrategy
{

    /// <summary>
    /// Indicates that the projection's state must be replaced with the define state
    /// </summary>
    public const string Replace = "replace";
    /// <summary>
    /// Indicates that the projection's state must be patched
    /// </summary>
    public const string Patch = "patch";

    /// <summary>
    /// Gets a new <see cref="IEnumerable{T}"/> that contains all supported values
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> that contains all supported values</returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return Replace;
        yield return Patch;
    }

}
