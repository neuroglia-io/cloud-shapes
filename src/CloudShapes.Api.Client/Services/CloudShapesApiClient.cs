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
