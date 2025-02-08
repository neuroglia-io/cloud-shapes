namespace CloudShapes.Api.Hubs;

/// <summary>
/// Represents the <see cref="Hub"/> used to observe <see cref="CloudEvent"/>s
/// </summary>
public class CloudEventHub
    : Hub<ICloudEventHubClient>, ICloudEventHub
{



}
