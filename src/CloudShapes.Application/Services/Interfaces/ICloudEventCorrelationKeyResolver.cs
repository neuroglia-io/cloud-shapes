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

namespace CloudShapes.Application.Services;

/// <summary>
/// Defines the fundamentals of a service used to resolve values from <see cref="CloudEvent"/>s
/// </summary>
public interface ICloudEventCorrelationKeyResolver
{

    /// <summary>
    /// Resolve the specified correlation key
    /// </summary>
    /// <param name="correlation">The definition of the correlation key to resolve</param>
    /// <param name="e">The <see cref="CloudEvent"/> to extract the correlation key from</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>The resolved value</returns>
    Task<string?> ResolveAsync(CloudEventCorrelationDefinition correlation, CloudEvent e, CancellationToken cancellationToken = default);

}
