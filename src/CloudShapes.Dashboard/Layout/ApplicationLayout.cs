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

namespace CloudShapes.Dashboard.Layout;

/// <summary>
/// Represents the default implementation of the <see cref="IApplicationLayout"/> interface
/// </summary>
public class ApplicationLayout
    : IApplicationLayout
{

    ApplicationTitle? _Title;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the application title's <see cref="RenderFragment"/>
    /// </summary>
    public RenderFragment? TitleFragment => this.Title?.ChildContent;

    /// <summary>
    /// Gets/sets the application's title
    /// </summary>
    public ApplicationTitle? Title
    {
        get => this._Title;
        set
        {
            if (this._Title == value) return;
            this._Title = value;
            this.OnTitleChanged();
        }
    }

    /// <inheritdoc/>
    public void OnTitleChanged()
    {
        if (this.PropertyChanged != null) this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Title)));
    }

}
