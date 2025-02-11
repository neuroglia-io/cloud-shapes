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
/// Represents the default implementation of the <see cref="ICloudEventCorrelationKeyResolver"/> interface
/// </summary>
/// <param name="expressionEvaluator">The service used to evaluate runtime expressions</param>
public class CloudEventCorrelationKeyResolver(IExpressionEvaluator expressionEvaluator)
    : ICloudEventCorrelationKeyResolver
{

    /// <summary>
    /// Gets the service used to evaluate runtime expressions
    /// </summary>
    protected IExpressionEvaluator ExpressionEvaluator { get; } = expressionEvaluator;

    /// <inheritdoc/>
    public virtual async Task<string?> ResolveAsync(CloudEventCorrelationDefinition correlation, CloudEvent e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(correlation);
        ArgumentNullException.ThrowIfNull(e);
        return correlation.Strategy switch
        {
            CloudEventValueResolutionStrategy.Attribute => (string?)e.GetAttribute(correlation.Attribute!),
            CloudEventValueResolutionStrategy.Expression => await this.ExpressionEvaluator.EvaluateAsync<string>(correlation.Expression!, e, cancellationToken: cancellationToken).ConfigureAwait(false),
            _ => throw new NotSupportedException($"The specified value resolution strategy '{correlation.Strategy}' is not supported")
        };
    }

}
