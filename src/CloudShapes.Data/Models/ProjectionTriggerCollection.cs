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
/// Represents a collection of triggers used to create, update and delete projections of a specific type
/// </summary>
public record ProjectionTriggerCollection
{

    /// <summary>
    /// Initializes a new <see cref="ProjectionTriggerCollection"/>
    /// </summary>
    public ProjectionTriggerCollection() { }

    /// <summary>
    /// Initializes a new <see cref="ProjectionTriggerCollection"/>
    /// </summary>
    /// <param name="create">A list containing the triggers responsible for creating new projections when specific CloudEvents occur</param>
    /// <param name="update">A list containing the triggers responsible for updating projections when specific CloudEvents occur</param>
    /// <param name="delete">A list containing the triggers responsible for deleting projections when specific CloudEvents occur</param>
    public ProjectionTriggerCollection(IEnumerable<CloudEventCreateTriggerDefinition> create, IEnumerable<CloudEventUpdateTriggerDefinition>? update = null, IEnumerable<CloudEventDeleteTriggerDefinition>? delete = null)
    {
        ArgumentNullException.ThrowIfNull(create);
        if (!create.Any()) throw new ArgumentOutOfRangeException(nameof(create), "The trigger collection must contain at least one trigger to define how projections are created");
        this.Create = new(create);
        this.Update = update == null ? null : new(update);
        this.Delete = delete == null ? null : new(delete);
    }

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for creating new projections when specific CloudEvents occur
    /// </summary>
    [YamlMember(Alias = "create")]
    public virtual EquatableList<CloudEventCreateTriggerDefinition> Create { get; set; } = [];

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for updating projections when specific CloudEvents occur
    /// </summary>
    [YamlMember(Alias = "update")]
    public virtual EquatableList<CloudEventUpdateTriggerDefinition>? Update { get; set; }

    /// <summary>
    /// Gets/sets a list containing the triggers responsible for deleting projections when specific CloudEvents occur
    /// </summary>
    [YamlMember(Alias = "delete")]
    public virtual EquatableList<CloudEventDeleteTriggerDefinition>? Delete { get; set; }

    /// <summary>
    /// Gets a new <see cref="IEnumerable{T}"/> containing all configured <see cref="CloudEventTriggerDefinition"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all configured <see cref="CloudEventTriggerDefinition"/>s</returns>
    public virtual IEnumerable<CloudEventTriggerDefinition> AsEnumerable()
    {
        foreach (var trigger in Create) yield return trigger;
        if (Update != null) foreach (var trigger in Update) yield return trigger;
        if (Delete != null) foreach (var trigger in Delete) yield return trigger;
    }

}