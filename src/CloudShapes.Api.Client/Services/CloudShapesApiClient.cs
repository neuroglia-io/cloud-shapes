namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Represents the default implementation of the <see cref="ICloudShapesApiClient"/> interface
/// </summary>
/// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
/// <param name="httpClient">The service used to perform HTTP requests</param>
public class CloudShapesApiClient(IServiceProvider serviceProvider, HttpClient httpClient)
    : ICloudShapesApiClient
{

    /// <inheritdoc/>
    public virtual IEventsApiClient Events { get; } = ActivatorUtilities.CreateInstance<EventsApiClient>(serviceProvider, httpClient);

    /// <inheritdoc/>
    public virtual IProjectionTypesApiClient ProjectionTypes { get; } = ActivatorUtilities.CreateInstance<ProjectionTypesApiClient>(serviceProvider, httpClient);

    /// <inheritdoc/>
    public virtual IProjectionsApiClient Projections { get; } = ActivatorUtilities.CreateInstance<ProjectionsApiClient>(serviceProvider, httpClient);

}
