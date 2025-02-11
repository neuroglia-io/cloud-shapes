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
/// Represents a JSON Patch operation
/// </summary>
public record JsonPatchOperation
{

    /// <summary>
    /// Gets/sets the path, if any, to copy from for the Move/Copy operation
    /// </summary>
    public virtual string? From { get; set; }

    /// <summary>
    /// Gets/sets the type of operation to perform
    /// </summary>
    [Required]
    public virtual string Op { get; set; } = null!;

    /// <summary>
    /// Gets/sets the path of the property to patch
    /// </summary>
    [Required]
    public virtual string Path { get; set; } = null!;

    /// <summary>
    /// Gets/sets the patched value
    /// </summary>
    public virtual JsonElement? Value { get; set; }

}
