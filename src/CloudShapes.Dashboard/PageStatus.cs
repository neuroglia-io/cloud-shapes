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

namespace CloudShapes.Dashboard;

/// <summary>
/// Represents a page's status
/// </summary>
public static class PageStatus
{
    /// <summary>
    /// Indicates that the page is currently loading data
    /// </summary>
    public const string Loading = "loading";
    /// <summary>
    /// Indicates that the page is waiting for user action
    /// </summary>
    public const string Pending = "pending";
    /// <summary>
    /// Indicates that the page is currently sending data to the server
    /// </summary>
    public const string Sending = "sending";
    /// <summary>
    /// Indicates that the page has completed its purpose (and awaiting redirection)
    /// </summary>
    public const string Completed = "loading";
}
