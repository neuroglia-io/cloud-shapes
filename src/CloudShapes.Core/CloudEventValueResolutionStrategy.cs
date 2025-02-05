namespace CloudShapes.Core;

/// <summary>
/// Enumerates all supported CloudEvent value resolution strategies
/// </summary>
public static class CloudEventValueResolutionStrategy
{

    /// <summary>
    /// Indicates that the value is extracted from a context attribute
    /// </summary>
    public const string Attribute = "attribute";
    /// <summary>
    /// Indicates that the value is resolved dynamically using a runtime expression
    /// </summary>
    public const string Expression = "expression";

    /// <summary>
    /// Gets a new <see cref="IEnumerable{T}"/> that contains all supported values
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> that contains all supported values</returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return Attribute;
        yield return Expression;
    }

}
