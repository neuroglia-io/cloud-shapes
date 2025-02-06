namespace CloudShapes.Application.Services;

/// <summary>
/// Defines the fundamentals of a service used to resolve values from <see cref="CloudEvent"/>s
/// </summary>
public interface ICloudEventCorrelationKeyResolver
{

    /// <summary>
    /// Resolve the specified correlation key
    /// </summary>
    /// <param name="correlation">The definition of the correlation key to resolve</param>
    /// <param name="e">The <see cref="CloudEvent"/> to extract the correlation key from</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resolved value</returns>
    Task<string?> ResolveAsync(CloudEventCorrelationDefinition correlation, CloudEvent e, CancellationToken cancellationToken = default);

}
