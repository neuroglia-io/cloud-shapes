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

namespace CloudShapes.Dashboard.Components.ProjectionDetailsStateManagement;

/// <summary>
/// Represents the state of a <see cref="ProjectionDetails"/>
/// </summary>
public record ProjectionDetailsState
{
    /// <summary>
    /// Gets/sets the projection
    /// </summary>
    public IDictionary<string, object>? Projection { get; set; } = null;

    /// <summary>
    /// Gets/sets the serialized projection
    /// </summary>
    public string SerializedProjection { get; set; } = string.Empty;

    /// <summary>
    /// Gets/sets the <see cref="ProjectionType"/> of the projection
    /// </summary>
    public ProjectionType? ProjectionType { get; set; } = null;

    /// <summary>
    /// Gets/sets a boolean indicating if the projection is being saved
    /// </summary>
    public bool IsSaving { get; set; } = false;

    /// <summary>
    /// Gets/sets a boolean indicating if the projection is being edited
    /// </summary>
    public bool IsEditing { get; set; } = false;

    /// <summary>
    /// Gets/sets the <see cref="ProblemDetails"/> type that occurred when trying to save the projection, if any
    /// </summary>
    public Uri? ProblemType { get; set; } = null;

    /// <summary>
    /// Gets/sets the <see cref="ProblemDetails"/> title that occurred when trying to save the projection, if any
    /// </summary>
    public string ProblemTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets/sets the <see cref="ProblemDetails"/> details that occurred when trying to save the projection, if any
    /// </summary>
    public string ProblemDetail { get; set; } = string.Empty;

    /// <summary>
    /// Gets/sets the <see cref="ProblemDetails"/> status that occurred when trying to save the projection, if any
    /// </summary>
    public int ProblemStatus { get; set; } = 0;

    /// <summary>
    /// Gets/sets the list of <see cref="ProblemDetails"/> errors that occurred when trying to save the projection, if any
    /// </summary>
    public IDictionary<string, string[]> ProblemErrors { get; set; } = new Dictionary<string, string[]>();
}
