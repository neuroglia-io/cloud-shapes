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
/// Represents the options used to configure the application's database
/// </summary>
public class DatabaseOptions
{

    /// <summary>
    /// Gets/sets the database's connection string
    /// </summary>
    public virtual string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Gets/sets the database's name
    /// </summary>
    public virtual string Name { get; set; } = "cloud-shapes";

    /// <summary>
    /// Gets/sets the options used to configure the provisioning, if any, of Cloud Shapes database
    /// </summary>
    public virtual DatabaseProvisioningOptions Provisioning { get; set; } = new();

}
