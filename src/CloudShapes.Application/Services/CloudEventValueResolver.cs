namespace CloudShapes.Application.Services;

/// <summary>
/// Represents the default implementation of the <see cref="ICloudEventValueResolver"/> interface
/// </summary>
/// <param name="expressionEvaluator">The service used to evaluate runtime expressions</param>
public class CloudEventValueResolver(IExpressionEvaluator expressionEvaluator)
    : ICloudEventValueResolver
{

    /// <summary>
    /// Gets the service used to evaluate runtime expressions
    /// </summary>
    protected IExpressionEvaluator ExpressionEvaluator { get; } = expressionEvaluator;

    /// <inheritdoc/>
    public virtual async Task<string?> ResolveAsync(CloudEventValueResolverDefinition value, CloudEvent e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(e);
        return value.Strategy switch
        {
            CloudEventValueResolutionStrategy.Attribute => (string?)e.GetAttribute(value.Attribute!),
            CloudEventValueResolutionStrategy.Expression => await this.ExpressionEvaluator.EvaluateAsync<string>(value.Expression!, e, cancellationToken: cancellationToken).ConfigureAwait(false),
            _ => throw new NotSupportedException($"The specified value resolution strategy '{value.Strategy}' is not supported")
        };
    }

}
