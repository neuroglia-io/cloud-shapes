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

namespace CloudShapes.Data.Models;

/// <summary>
/// Represents the definition of a CloudEvent filter
/// </summary>
public record CloudEventFilterDefinition
{

    /// <summary>
    /// Initializes a new <see cref="CloudEventFilterDefinition"/>
    /// </summary>
    public CloudEventFilterDefinition() { }

    /// <summary>
    /// Initializes a new <see cref="CloudEventFilterDefinition"/>
    /// </summary>
    /// <param name="source">The source uri of filtered CloudEvents. Supports Regular Expressions.</param>
    /// <param name="type">The type of filtered CloudEvents. Supports Regular Expressions.</param>
    /// <param name="correlation">An object used to configure how to resolve the correlation of filtered CloudEvents</param>
    public CloudEventFilterDefinition(string? source, string type, CloudEventCorrelationDefinition correlation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentNullException.ThrowIfNull(correlation);
        this.Source = source;
        this.Type = type;
        this.Correlation = correlation;
    }

    /// <summary>
    /// Gets/sets the source uri of filtered CloudEvents. Supports Regular Expressions.
    /// </summary>
    public virtual string? Source { get; set; }

    /// <summary>
    /// Gets/sets the type of filtered CloudEvents. Supports Regular Expressions.
    /// </summary>
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets an object used to configure how to resolve the correlation id of filtered CloudEvents
    /// </summary>
    public virtual CloudEventCorrelationDefinition Correlation { get; set; } = new();

}
