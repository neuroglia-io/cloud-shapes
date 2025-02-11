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
/// Exposes the default breadcrumbs for all pages of the application
/// </summary>
public static class Breadcrumbs
{

    /// <summary>
    /// Holds the breadcrumb items for about related routes
    /// </summary>
    public static readonly BreadcrumbItem[] About = [new("About", "/about")];

}
