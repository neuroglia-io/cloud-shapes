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

namespace CloudShapes.Application.Configuration;

/// <summary>
/// Represents the options used to configure the application
/// </summary>
public class ApplicationOptions
{

    /// <summary>
    /// Gets/sets the options used to configure the application's database
    /// </summary>
    public virtual DatabaseOptions Database { get; set; } = new();

    /// <summary>
    /// Gets/sets the options used to configure the application's <see cref="CloudEvent"/>s
    /// </summary>
    public virtual CloudEventOptions Events { get; set; } = new();

}
