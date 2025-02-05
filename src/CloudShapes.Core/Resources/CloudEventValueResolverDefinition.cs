namespace CloudShapes.Core.Resources;

/// <summary>
/// Defines how to extract a specific value from a CloudEvent
/// </summary>
public record CloudEventValueResolverDefinition
{

    /// <summary>
    /// Gets/sets the name of the attribute to extract the value from<para></para>
    /// Required when the strategy is set to <see cref="CloudEventValueResolutionStrategy.Attribute"/>, otherwise ignored
    /// </summary>
    public virtual string? Attribute { get; set; }

    /// <summary>
    /// Gets/sets a runtime expression used to resolve the value dynamically<para></para>
    /// Required when the strategy is set to <see cref="CloudEventValueResolutionStrategy.Expression"/>, otherwise ignored
    /// </summary>
    public virtual string? Expression { get; set; }

    /// <summary>
    /// Gets the strategy used to extract the value from the CloudEvent
    /// </summary>
    public virtual string Strategy => string.IsNullOrWhiteSpace(this.Attribute) ? string.IsNullOrWhiteSpace(this.Expression) ? throw new Exception("Either 'attribute' or 'expression' must be defined") : CloudEventValueResolutionStrategy.Expression : CloudEventValueResolutionStrategy.Attribute;

    /// <summary>
    /// Creates a new <see cref="CloudEventValueResolverDefinition"/> used to extract a value from a CloudEvent context attribute
    /// </summary>
    /// <param name="attribute">The name of the CloudEvent context attribute to extract the value from</param>
    /// <returns>A new <see cref="CloudEventValueResolverDefinition"/></returns>
    public static CloudEventValueResolverDefinition FromAttribute(string attribute) => new() { Attribute = attribute };

    /// <summary>
    /// Creates a new <see cref="CloudEventValueResolverDefinition"/> used to resolve a value using a runtime expression
    /// </summary>
    /// <param name="expression">The runtime expression used to resolve the value</param>
    /// <returns>A new <see cref="CloudEventValueResolverDefinition"/></returns>
    public static CloudEventValueResolverDefinition FromExpression(string expression) => new() { Expression = expression };

}