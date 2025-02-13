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

namespace CloudShapes.Integration.Models;

/// <summary>
/// Represents the options used to configure a specific query
/// </summary>
public record QueryOptions
{

    /// <summary>
    /// Gets/sets maximum number of items to return per page. If null, no limit is applied.
    /// </summary>
    public virtual int? Limit { get; set; }

    /// <summary>
    /// Gets/sets the number of items to skip, used for pagination.
    /// </summary>
    public virtual int? Skip { get; set; }

    /// <summary>
    /// Gets/sets the term to search for, if any
    /// </summary>
    public virtual string? Search { get; set; }

    /// <summary>
    /// Gets/sets the property to order items by, if any
    /// </summary>
    public virtual string? OrderBy { get; set; }

    /// <summary>
    /// Gets/sets a boolean that defines whether or not to order items in a descending fashion. Ignored if <see cref="OrderBy"/> has not been set
    /// </summary>
    public virtual bool Descending { get; set; }

    /// <summary>
    /// Gets/sets a dictionary of filters where the key is the field name and the value is the expected value.
    /// </summary>
    public virtual Dictionary<string, string>? Filters { get; set; }

}
