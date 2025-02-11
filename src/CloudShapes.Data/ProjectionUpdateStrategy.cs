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

namespace CloudShapes.Data;

/// <summary>
/// Enumerates all supported projection update strategies
/// </summary>
public static class ProjectionUpdateStrategy
{

    /// <summary>
    /// Indicates that the projection's state must be replaced with the define state
    /// </summary>
    public const string Replace = "replace";
    /// <summary>
    /// Indicates that the projection's state must be patched
    /// </summary>
    public const string Patch = "patch";

    /// <summary>
    /// Gets a new <see cref="IEnumerable{T}"/> that contains all supported values
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> that contains all supported values</returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return Replace;
        yield return Patch;
    }

}
