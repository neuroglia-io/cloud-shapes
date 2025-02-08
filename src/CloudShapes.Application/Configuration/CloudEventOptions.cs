namespace CloudShapes.Application.Configuration;

/// <summary>
/// Represents the options used to configure the application's <see cref="CloudEvent"/>s
/// </summary>
public class CloudEventOptions
{

    /// <summary>
    /// /Gets/sets the source for all <see cref="CloudEvent"/>s produced by the application
    /// </summary>
    public virtual Uri Source { get; set; } = new(CloudEvents.DefaultSource);

}