﻿// Copyright © 2025-Present The Cloud Shapes Authors
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

namespace CloudShapes.Integration.Queries.ProjectionTypes;

/// <summary>
/// Represents the query used to get a specific projection type
/// </summary>
/// <param name="name">The name of the projection type to get</param>
public class GetProjectionTypeQuery(string name)
    : Query<ProjectionType>
{

    /// <summary>
    /// Gets the name of the projection to get
    /// </summary>
    public virtual string Name { get; } = name;

}
