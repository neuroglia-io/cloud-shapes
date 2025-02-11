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
/// Represents the metadata of a projection type document
/// </summary>
public record ProjectionTypeMetadata
    : DocumentMetadata
{

    /// <summary>
    /// Gets/sets the total count of projections of this type
    /// </summary>
    public virtual long ProjectionCount { get; set; }

}
