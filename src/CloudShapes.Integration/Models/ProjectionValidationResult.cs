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
/// Represents the result of a projection's validation
/// </summary>
/// <param name="Id">The id of the validated projection</param>
/// <param name="Errors">A key/values mapping containing the errors, if any, that have occurred during the projection's validation</param>
public record ProjectionValidationResult(string Id, EquatableDictionary<string, string[]>? Errors = null)
{

    /// <summary>
    /// Gets a boolean indicating whether or not the projection is valid
    /// </summary>
    [JsonIgnore]
    public virtual bool IsValid => Errors == null || Errors.Count < 1;

}