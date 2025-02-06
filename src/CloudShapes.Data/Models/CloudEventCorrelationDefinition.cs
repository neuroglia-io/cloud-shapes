namespace CloudShapes.Data.Models;

/// <summary>
/// Represents an object used to define and configure a CloudEvent correlation
/// </summary>
public record CloudEventCorrelationDefinition
{

    /// <summary>
    /// Gets the name of the default projection correlation key
    /// </summary>
    public const string DefaultKey = "_id";

    /// <summary>
    /// Gets/sets the path to the projection's correlation key. Defaults to <see cref="DefaultKey"/>
    /// </summary>
    public virtual string Key { get; set; } = DefaultKey;

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
    /// Creates a new <see cref="CloudEventCorrelationDefinition"/> used to extract a value from a CloudEvent context attribute
    /// </summary>
    /// <param name="attribute">The name of the CloudEvent context attribute to extract the value from</param>
    /// <param name="key">The path to the projection's correlation key. Defaults to <see cref="DefaultKey"/></param>
    /// <returns>A new <see cref="CloudEventCorrelationDefinition"/></returns>
    public static CloudEventCorrelationDefinition FromAttribute(string attribute, string key = DefaultKey) => new() { Attribute = attribute, Key = key };

    /// <summary>
    /// Creates a new <see cref="CloudEventCorrelationDefinition"/> used to resolve a value using a runtime expression
    /// </summary>
    /// <param name="expression">The runtime expression used to resolve the value</param>
    /// <param name="key">The path to the projection's correlation key. Defaults to <see cref="DefaultKey"/></param>
    /// <returns>A new <see cref="CloudEventCorrelationDefinition"/></returns>
    public static CloudEventCorrelationDefinition FromExpression(string expression, string key = DefaultKey) => new() { Expression = expression, Key = key };

}