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

namespace CloudShapes.Application.Configuration;

/// <summary>
/// Represents the options used to configure the provisioning, if any, of Cloud Shapes database
/// </summary>
public class DatabaseProvisioningOptions
{

    /// <summary>
    /// Gets the path to the directory from which to load the static resources used to seed the database
    /// </summary>
    public static readonly string DefaultDirectory = Path.Combine(AppContext.BaseDirectory, "data", "seed");
    /// <summary>
    /// Gets the default GLOB pattern used to match the static resource files to use to seed the database
    /// </summary>
    public const string DefaultFilePattern = "*.*";

    /// <summary>
    /// Gets/sets the directory from which to load the static resources used to seed the database
    /// </summary>
    public virtual string Directory { get; set; } = DefaultDirectory;

    /// <summary>
    /// Gets/sets a boolean indicating whether or not to overwrite existing resources
    /// </summary>
    public virtual bool Overwrite { get; set; }

    /// <summary>
    /// Gets/sets the GLOB pattern used to match the static resource files to use to seed the database
    /// </summary>
    public virtual string FilePattern { get; set; } = DefaultFilePattern;

}
