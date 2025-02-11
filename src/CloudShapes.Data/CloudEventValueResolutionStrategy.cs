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
/// Enumerates all supported CloudEvent value resolution strategies
/// </summary>
public static class CloudEventValueResolutionStrategy
{

    /// <summary>
    /// Indicates that the value is extracted from a context attribute
    /// </summary>
    public const string Attribute = "attribute";
    /// <summary>
    /// Indicates that the value is resolved dynamically using a runtime expression
    /// </summary>
    public const string Expression = "expression";

    /// <summary>
    /// Gets a new <see cref="IEnumerable{T}"/> that contains all supported values
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> that contains all supported values</returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return Attribute;
        yield return Expression;
    }

}
