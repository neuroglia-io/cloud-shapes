namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Defines the fundamentals of a service used to interact with the Cloud Shapes API
/// </summary>
public interface ICloudShapesApiClient
{

    /// <summary>
    /// Gets the service used to interact with the Cloud Shapes Events API
    /// </summary>
    IEventsApiClient Events { get; }

    /// <summary>
    /// Gets the service used to interact with the Cloud Shapes Projection Types API
    /// </summary>
    IProjectionTypesApiClient ProjectionTypes { get; }

    /// <summary>
    /// Gets the service used to interact with the Cloud Shapes Projections API
    /// </summary>
    IProjectionsApiClient Projections { get; }

}
