namespace CloudShapes.Data.Models;

/// <summary>
/// Represents the definition of a CloudEvent filter
/// </summary>
public record CloudEventFilterDefinition
{

    /// <summary>
    /// Initializes a new <see cref="CloudEventFilterDefinition"/>
    /// </summary>
    public CloudEventFilterDefinition() { }

    /// <summary>
    /// Initializes a new <see cref="CloudEventFilterDefinition"/>
    /// </summary>
    /// <param name="source">The source uri of filtered CloudEvents. Supports Regular Expressions.</param>
    /// <param name="type">The type of filtered CloudEvents. Supports Regular Expressions.</param>
    /// <param name="correlation">An object used to configure how to resolve the correlation of filtered CloudEvents</param>
    public CloudEventFilterDefinition(string? source, string type, CloudEventCorrelationDefinition correlation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentNullException.ThrowIfNull(correlation);
        this.Source = source;
        this.Type = type;
        this.Correlation = correlation;
    }

    /// <summary>
    /// Gets/sets the source uri of filtered CloudEvents. Supports Regular Expressions.
    /// </summary>
    public virtual string? Source { get; set; }

    /// <summary>
    /// Gets/sets the type of filtered CloudEvents. Supports Regular Expressions.
    /// </summary>
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets an object used to configure how to resolve the correlation id of filtered CloudEvents
    /// </summary>
    public virtual CloudEventCorrelationDefinition Correlation { get; set; } = null!;

}
