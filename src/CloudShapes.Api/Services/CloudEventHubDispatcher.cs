// Copyright © 2025-Present The Cloud Shapes Authors
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
