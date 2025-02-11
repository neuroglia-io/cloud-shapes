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
/// Represents the definition of a projection's index
/// </summary>
public record ProjectionIndexDefinition
{

    /// <summary>
    /// Gets/sets the index's name
    /// </summary>
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing the properties on which the index is defined
    /// </summary>
    public virtual EquatableList<string> Properties { get; set; } = [];

    /// <summary>
    /// Gets/sets a boolean that defines whether or not the index is unique
    /// </summary>
    public virtual bool Unique { get; set; }

    /// <summary>
    /// Gets/sets a boolean that defines whether or not the index is sorted in a descending fashion
    /// </summary>
    public virtual bool Descending { get; set; }

    /// <summary>
    /// Gets/sets a boolean that defines whether or not the index supports full text searches
    /// </summary>
    public virtual bool Text { get; set; }

}
