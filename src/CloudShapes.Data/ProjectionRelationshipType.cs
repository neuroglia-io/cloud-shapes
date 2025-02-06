namespace CloudShapes.Data;

/// <summary>
/// Enumerates all supported CloudEvent value resolution strategies
/// </summary>
public static class ProjectionRelationshipType
{

    /// <summary>
    /// Indicates a relationship where the projection is associated with a single related projection
    /// </summary>
    public const string OneToOne = "one-to-one";
    /// <summary>
    /// Indicates a relationship where the projection is associated with multiple related projections
    /// </summary>
    public const string OneToMany = "one-to-many";

    /// <summary>
    /// Gets a new <see cref="IEnumerable{T}"/> that contains all supported values
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> that contains all supported values</returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return OneToOne;
        yield return OneToMany;
    }

}