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

namespace CloudShapes.Integration.Commands.Projections;

/// <summary>
/// Represents the command used to patch a projection
/// </summary>
/// <param name="type">The name of the type of projection to patch</param>
/// <param name="id">The id of the projection to patch</param>
/// <param name="patch">The patch to apply</param>
public class PatchProjectionCommand(string type, string id, Patch patch)
    : Command<IDictionary<string, object>>
{

    /// <summary>
    /// Gets the name of the type of projection to patch
    /// </summary>
    public virtual string Type { get; } = type;

    /// <summary>
    /// Gets/sets the id of the projection to patch
    /// </summary>
    public virtual string Id { get; } = id;

    /// <summary>
    /// Gets the patch to apply
    /// </summary>
    public virtual Patch Patch { get; } = patch;

}