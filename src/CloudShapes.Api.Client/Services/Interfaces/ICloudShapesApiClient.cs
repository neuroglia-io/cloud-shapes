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

namespace CloudShapes.Api.Client.Services;

/// <summary>
/// Defines the fundamentals of a service used to interact with the Cloud Shapes API
/// </summary>
public interface ICloudShapesApiClient
{

    /// <summary>
    /// Gets the service used to interact with the Cloud Shapes Events API
    /// </summary>
    IEventsApiClient Events { get; }

    /// <summary>
    /// Gets the service used to interact with the Cloud Shapes Projection Types API
    /// </summary>
    IProjectionTypesApiClient ProjectionTypes { get; }

    /// <summary>
    /// Gets the service used to interact with the Cloud Shapes Projections API
    /// </summary>
    IProjectionsApiClient Projections { get; }

}
