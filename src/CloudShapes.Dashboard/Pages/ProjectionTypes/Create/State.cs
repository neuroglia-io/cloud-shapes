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

using CloudShapes.Integration.Commands.ProjectionTypes;

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.Create;

/// <summary>
/// Represents the state of the create projection type view
/// </summary>
public record CreateProjectionTypeState
{

    /// <summary>
    /// Gets/sets a boolean indicating whether or not the view is loading
    /// </summary>
    public bool Loading { get; set; }

    /// <summary>
    /// Gets/sets the command used to create a new projection type
    /// </summary>
    public CreateProjectionTypeCommand Command { get; set; } = new();

    /// <summary>
    /// Gets/sets a list containing the errors that have occurred during the validation or the creation of the projection type
    /// </summary>
    public EquatableList<string> Errors { get; set; } = [];

}
