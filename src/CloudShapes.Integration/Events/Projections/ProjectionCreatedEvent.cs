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

namespace CloudShapes.Integration.Events.Projections;

/// <summary>
/// Represents the payload of a <see cref="CloudEvent"/> published whenever a new projection has been created
/// </summary>
/// <param name="Id">The id of the newly created projection</param>
/// <param name="Type">The name of the newly created projection's type</param>
/// <param name="State">The newly created projection's initial state</param>
public record ProjectionCreatedEvent(string Id, string Type, object State);
