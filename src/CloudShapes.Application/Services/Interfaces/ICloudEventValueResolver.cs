namespace CloudShapes.Application.Services;

/// <summary>
/// Defines the fundamentals of a service used to resolve values from <see cref="CloudEvent"/>s
/// </summary>
public interface ICloudEventValueResolver
{

    /// <summary>
    /// Resolve the specified value
    /// </summary>
    /// <param name="value">The definition of the value to resolve</param>
    /// <param name="e">The <see cref="CloudEvent"/> to resolve the value from</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resolved value</returns>
    Task<string?> ResolveAsync(CloudEventValueResolverDefinition value, CloudEvent e, CancellationToken cancellationToken = default);

}
