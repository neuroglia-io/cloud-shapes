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

namespace CloudShapes.Dashboard.Components;

/// <summary>
/// The change event emitted when the user selects a different page size or navigates to a different page
/// </summary>
/// <param name="pageIndex">The zero-based index of the current page.</param>
/// <param name="pageSize">The number of items per page.</param>
/// <param name="previousPageIndex">The zero-based index of the previous page.</param>
public class PageEvent(int pageIndex, int pageSize, int previousPageIndex)
{

    /// <summary>
    /// Gets/sets the zero-based index of the current page.
    /// </summary>
    public int PageIndex { get; set; } = pageIndex;

    /// <summary>
    /// Gets/sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = pageSize;

    /// <summary>
    /// Gets/sets the zero-based index of the previous page.
    /// </summary>
    public int PreviousPageIndex { get; set; } = previousPageIndex;
}
