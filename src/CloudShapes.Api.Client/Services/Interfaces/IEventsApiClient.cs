namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Defines the fundamentals of a service used to interact with the Cloud Shapes Events API
/// </summary>
public interface IEventsApiClient
{

    /// <summary>
    /// Publishes the specified <see cref="CloudEvent"/>
    /// </summary>
    /// <param name="e">The <see cref="CloudEvent"/> to publish</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task PublishAsync(CloudEvent e, CancellationToken cancellationToken = default);

}
