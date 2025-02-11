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

namespace CloudShapes.Data.Models;

/// <summary>
/// Represents an object that holds information about a specific document
/// </summary>
public record DocumentMetadata
{

    /// <summary>
    /// Gets the name of the BSON property used to store a document's metadata
    /// </summary>
    public const string PropertyName = "_metadata";

    /// <summary>
    /// Gets/sets the date and time the document was created at
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets/sets the date and time the document was last modified at
    /// </summary>
    public virtual DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// Gets/sets the document's version number
    /// </summary>
    public virtual int Version { get; set; } = 1;

    /// <summary>
    /// Updates the <see cref="DocumentMetadata"/>
    /// </summary>
    public virtual void Update()
    {
        LastModified = DateTimeOffset.Now;
        Version++;
    }

}
