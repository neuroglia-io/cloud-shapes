namespace CloudShapes.Application.Services;

/// <summary>
/// Represents the default implementation of the <see cref="ICloudEventCorrelationKeyResolver"/> interface
/// </summary>
/// <param name="expressionEvaluator">The service used to evaluate runtime expressions</param>
public class CloudEventCorrelationKeyResolver(IExpressionEvaluator expressionEvaluator)
    : ICloudEventCorrelationKeyResolver
{

    /// <summary>
    /// Gets the service used to evaluate runtime expressions
    /// </summary>
    protected IExpressionEvaluator ExpressionEvaluator { get; } = expressionEvaluator;

    /// <inheritdoc/>
    public virtual async Task<string?> ResolveAsync(CloudEventCorrelationDefinition correlation, CloudEvent e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(correlation);
        ArgumentNullException.ThrowIfNull(e);
        return correlation.Strategy switch
        {
            CloudEventValueResolutionStrategy.Attribute => (string?)e.GetAttribute(correlation.Attribute!),
            CloudEventValueResolutionStrategy.Expression => await this.ExpressionEvaluator.EvaluateAsync<string>(correlation.Expression!, e, cancellationToken: cancellationToken).ConfigureAwait(false),
            _ => throw new NotSupportedException($"The specified value resolution strategy '{correlation.Strategy}' is not supported")
        };
    }

}
