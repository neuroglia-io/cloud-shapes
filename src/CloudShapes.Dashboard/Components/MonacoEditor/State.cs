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

namespace CloudShapes.Dashboard.Components.MonacoEditorStateManagement;

/// <summary>
/// Represents the state of a <see cref="MonacoEditor"/>
/// </summary>
public record MonacoEditorState
{

    /// <summary>
    /// Gets/sets the text representation of the referenced document
    /// </summary>
    public string DocumentText { get; set; } = string.Empty;

    /// <summary>
    /// Gets/sets a boolean indicating the editor is read-only
    /// </summary>
    public bool IsReadOnly { get; set; } = false;

    /// <summary>
    /// Gets/sets the document's model name, if any
    /// </summary>
    public string ModelName { get; set; } = string.Empty;

}
