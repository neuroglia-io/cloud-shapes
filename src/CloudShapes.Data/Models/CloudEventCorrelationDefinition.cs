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
/// Represents an object used to define and configure a CloudEvent correlation
/// </summary>
public record CloudEventCorrelationDefinition
{

    /// <summary>
    /// Gets the name of the default projection correlation key
    /// </summary>
    public const string DefaultKey = "_id";

    /// <summary>
    /// Gets/sets the path to the projection's correlation key. Defaults to <see cref="DefaultKey"/>
    /// </summary>
    public virtual string Key { get; set; } = DefaultKey;

    /// <summary>
    /// Gets/sets the name of the attribute to extract the value from<para></para>
    /// Required when the strategy is set to <see cref="CloudEventValueResolutionStrategy.Attribute"/>, otherwise ignored
    /// </summary>
    public virtual string? Attribute { get; set; }

    /// <summary>
    /// Gets/sets a runtime expression used to resolve the value dynamically<para></para>
    /// Required when the strategy is set to <see cref="CloudEventValueResolutionStrategy.Expression"/>, otherwise ignored
    /// </summary>
    public virtual string? Expression { get; set; }

    /// <summary>
    /// Gets the strategy used to extract the value from the CloudEvent
    /// </summary>
    [JsonIgnore]
    public virtual string Strategy => string.IsNullOrWhiteSpace(this.Attribute) ? string.IsNullOrWhiteSpace(this.Expression) ? throw new Exception("Either 'attribute' or 'expression' must be defined") : CloudEventValueResolutionStrategy.Expression : CloudEventValueResolutionStrategy.Attribute;

    /// <summary>
    /// Creates a new <see cref="CloudEventCorrelationDefinition"/> used to extract a value from a CloudEvent context attribute
    /// </summary>
    /// <param name="attribute">The name of the CloudEvent context attribute to extract the value from</param>
    /// <param name="key">The path to the projection's correlation key. Defaults to <see cref="DefaultKey"/></param>
    /// <returns>A new <see cref="CloudEventCorrelationDefinition"/></returns>
    public static CloudEventCorrelationDefinition FromAttribute(string attribute, string key = DefaultKey) => new() { Attribute = attribute, Key = key };

    /// <summary>
    /// Creates a new <see cref="CloudEventCorrelationDefinition"/> used to resolve a value using a runtime expression
    /// </summary>
    /// <param name="expression">The runtime expression used to resolve the value</param>
    /// <param name="key">The path to the projection's correlation key. Defaults to <see cref="DefaultKey"/></param>
    /// <returns>A new <see cref="CloudEventCorrelationDefinition"/></returns>
    public static CloudEventCorrelationDefinition FromExpression(string expression, string key = DefaultKey) => new() { Expression = expression, Key = key };

}