namespace CloudShapes.Api.Services;

/// <summary>
/// Represents a service used to dispatch ingested cloud events to all <see cref="ICloudEventHub"/>s
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="cloudEventBus">The service used to observe both inbound and outbound <see cref="CloudEvent"/>s</param>
/// <param name="hubContext">The current <see cref="ICloudEventHubClient"/>'s <see cref="IHubContext{THub, T}"/></param>
public class CloudEventHubDispatcher(ILogger<CloudEventHubDispatcher> logger, ICloudEventBus cloudEventBus, IHubContext<CloudEventHub, ICloudEventHubClient> hubContext)
    : BackgroundService
{

    IDisposable? _subscriptionHandle;

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; } = logger;

    /// <summary>
    /// Gets the service used to observe both inbound and outbound <see cref="CloudEvent"/>s
    /// </summary>
    protected ICloudEventBus CloudEventBus => cloudEventBus;

    /// <summary>
    /// Gets the current <see cref="ICloudEventHubClient"/>'s <see cref="IHubContext{THub, T}"/>
    /// </summary>
    protected IHubContext<CloudEventHub, ICloudEventHubClient> HubContext { get; } = hubContext;

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _subscriptionHandle = this.CloudEventBus.OutputStream.SubscribeAsync(e => this.HubContext.Clients.All.StreamEvent(e, stoppingToken));
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        base.Dispose();
        _subscriptionHandle?.Dispose();
    }

}
