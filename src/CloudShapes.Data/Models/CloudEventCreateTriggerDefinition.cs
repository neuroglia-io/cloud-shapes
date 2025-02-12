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
/// Represents a trigger that creates a new projection when a CloudEvent occurs
/// </summary>
public record CloudEventCreateTriggerDefinition 
    : CloudEventTriggerDefinition
{

    /// <summary>
    /// Gets/sets a an object that represents the projection's initial state<para></para>
    /// Supports runtime expressions
    /// </summary>
    [YamlMember(Alias = "state")]
    public virtual object State { get; set; } = null!;

}