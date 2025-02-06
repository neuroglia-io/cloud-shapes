namespace CloudShapes.Api.Client.Configuration;

/// <summary>
/// Represents the options used to configure a Cloud Shapes API client
/// </summary>
public class CloudShapesApiClientOptions
{

    /// <summary>
    /// Gets/sets the base address of the Cloud Shapes API to connect to
    /// </summary>
    public virtual Uri BaseAddress { get; set; } = null!;

}
