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

namespace CloudShapes.Application.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage projection repositories
/// </summary>
public interface IDbContext
{

    /// <summary>
    /// Gets the <see cref="IRepository"/> used to manage projections of the specified type
    /// </summary>
    /// <param name="typeName">The name of the type of projection to get the repository for</param>
    /// <returns>The <see cref="IRepository"/> used to manage projections of the specified type</returns>
    IRepository Set(string typeName);

    /// <summary>
    /// Gets the <see cref="IRepository"/> used to manage projections of the specified type
    /// </summary>
    /// <param name="type">The type of projection to get the repository for</param>
    /// <returns>The <see cref="IRepository"/> used to manage projections of the specified type</returns>
    IRepository Set(ProjectionType type);

}
