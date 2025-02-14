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
/// Represents the command used to create a new projection
/// </summary>
/// <param name="type">The name of the type of projection to create</param>
/// <param name="id">The id of the projection to create</param>
/// <param name="state">The initial state of the projection to create</param>
public class CreateProjectionCommand(string type, string id, IDictionary<string, object> state)
    : Command<IDictionary<string, object>>
{

    /// <summary>
    /// Gets the name of the type of projection to create
    /// </summary>
    public virtual string Type { get; } = type;

    /// <summary>
    /// Gets/sets the id of the projection to create
    /// </summary>
    public virtual string Id { get; } = id;

    /// <summary>
    /// Gets the initial state of the projection to create
    /// </summary>
    public virtual IDictionary<string, object> State { get; } = state;

}
