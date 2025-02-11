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

namespace CloudShapes;

/// <summary>
/// Exposes constants about the <see cref="CloudEvent"/>s consumed and produced by Cloud Shapes
/// </summary>
public static class CloudEvents
{

    /// <summary>
    /// Gets the default source for all <see cref="CloudEvent"/>s produced by Cloud Shapes
    /// </summary>
    public const string DefaultSource = "https://cloud-shapes.io";
    /// <summary>
    /// Gets the prefix for the type of all <see cref="CloudEvent"/>s produced by Cloud Shapes
    /// </summary>
    public const string TypePrefix = "io.cloud-shapes.events";

    /// <summary>
    /// Exposes constants about projection type-related <see cref="CloudEvent"/>s
    /// </summary>
    public static class ProjectionTypes
    {

        /// <summary>
        /// Gets the prefix for the type of all projection type-related <see cref="CloudEvent"/>s produced by Cloud Shapes
        /// </summary>
        public static readonly string TypePrefix = $"{CloudEvents.TypePrefix}.projection-type";

        /// <summary>
        /// Exposes constants about the <see cref="CloudEvent"/> used to notify about the creation of a projection type
        /// </summary>
        public static class Created
        {

            /// <summary>
            /// Gets the prefix for the type of all versions of <see cref="CloudEvent"/>s used to notify about the creation of a projection type
            /// </summary>
            public static readonly string TypePrefix = $"{Projections.TypePrefix}.created";

            /// <summary>
            /// Exposes constants about the v1 of the <see cref="CloudEvent"/> used to notify about the creation of a projection type
            /// </summary>
            public static class V1
            {

                /// <summary>
                /// Gets the type of the v1 of the <see cref="CloudEvent"/> used to notify about the creation of a projection type
                /// </summary>
                public static readonly string Type = $"{TypePrefix}.v1";

            }

            /// <summary>
            /// Gets all versions of the projection type created event
            /// </summary>
            /// <returns>A new <see cref="IEnumerable{T}"/> containing all versions of the projection created event</returns>
            public static IEnumerable<string> GetVersions()
            {
                yield return V1.Type;
            }

        }

        /// <summary>
        /// Exposes constants about the <see cref="CloudEvent"/> used to notify about the update of a projection type
        /// </summary>
        public static class Updated
        {

            /// <summary>
            /// Gets the prefix for the type of all versions of <see cref="CloudEvent"/>s used to notify about the update of a projection type
            /// </summary>
            public static readonly string TypePrefix = $"{Projections.TypePrefix}.updated";

            /// <summary>
            /// Exposes constants about the v1 of the <see cref="CloudEvent"/> used to notify about the update of a projection type
            /// </summary>
            public static class V1
            {

                /// <summary>
                /// Gets the type of the v1 of the <see cref="CloudEvent"/> used to notify about the update of a projection type
                /// </summary>
                public static readonly string Type = $"{TypePrefix}.v1";

            }

            /// <summary>
            /// Gets all versions of the projection type updated event
            /// </summary>
            /// <returns>A new <see cref="IEnumerable{T}"/> containing all versions of the projection updated event</returns>
            public static IEnumerable<string> GetVersions()
            {
                yield return V1.Type;
            }

        }

        /// <summary>
        /// Exposes constants about the <see cref="CloudEvent"/> used to notify about the deletion of a projection type
        /// </summary>
        public static class Deleted
        {

            /// <summary>
            /// Gets the prefix for the type of all versions of <see cref="CloudEvent"/>s used to notify about the deletion of a projection type
            /// </summary>
            public static readonly string TypePrefix = $"{Projections.TypePrefix}.deleted";

            /// <summary>
            /// Exposes constants about the v1 of the <see cref="CloudEvent"/> used to notify about the deletion of a projection type
            /// </summary>
            public static class V1
            {

                /// <summary>
                /// Gets the type of the v1 of the <see cref="CloudEvent"/> used to notify about the deletion of a projection type
                /// </summary>
                public static readonly string Type = $"{TypePrefix}.v1";

            }

            /// <summary>
            /// Gets all versions of the projection type deleted event
            /// </summary>
            /// <returns>A new <see cref="IEnumerable{T}"/> containing all versions of the projection deleted event</returns>
            public static IEnumerable<string> GetVersions()
            {
                yield return V1.Type;
            }

        }

        /// <summary>
        /// Gets all types of projection type-related <see cref="CloudEvent"/>s
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all types of projection type-related <see cref="CloudEvent"/>s</returns>
        public static IEnumerable<string> GetTypes()
        {
            foreach (var version in Created.GetVersions()) yield return version;
            foreach (var version in Updated.GetVersions()) yield return version;
            foreach (var version in Deleted.GetVersions()) yield return version;
        }

    }

    /// <summary>
    /// Exposes constants about projection-related <see cref="CloudEvent"/>s
    /// </summary>
    public static class Projections
    {

        /// <summary>
        /// Gets the prefix for the type of all projection-related <see cref="CloudEvent"/>s produced by Cloud Shapes
        /// </summary>
        public static readonly string TypePrefix = $"{CloudEvents.TypePrefix}.projection";

        /// <summary>
        /// Exposes constants about the <see cref="CloudEvent"/> used to notify about the creation of a projection
        /// </summary>
        public static class Created
        {

            /// <summary>
            /// Gets the prefix for the type of all versions of <see cref="CloudEvent"/>s used to notify about the creation of a projection
            /// </summary>
            public static readonly string TypePrefix = $"{Projections.TypePrefix}.created";

            /// <summary>
            /// Exposes constants about the v1 of the <see cref="CloudEvent"/> used to notify about the creation of a projection
            /// </summary>
            public static class V1
            {

                /// <summary>
                /// Gets the type of the v1 of the <see cref="CloudEvent"/> used to notify about the creation of a projection
                /// </summary>
                public static readonly string Type = $"{TypePrefix}.v1";

            }

            /// <summary>
            /// Gets all versions of the projection created event
            /// </summary>
            /// <returns>A new <see cref="IEnumerable{T}"/> containing all versions of the projection created event</returns>
            public static IEnumerable<string> GetVersions()
            {
                yield return V1.Type;
            }

        }

        /// <summary>
        /// Exposes constants about the <see cref="CloudEvent"/> used to notify about the update of a projection
        /// </summary>
        public static class Updated
        {

            /// <summary>
            /// Gets the prefix for the type of all versions of <see cref="CloudEvent"/>s used to notify about the update of a projection
            /// </summary>
            public static readonly string TypePrefix = $"{Projections.TypePrefix}.updated";

            /// <summary>
            /// Exposes constants about the v1 of the <see cref="CloudEvent"/> used to notify about the update of a projection
            /// </summary>
            public static class V1
            {

                /// <summary>
                /// Gets the type of the v1 of the <see cref="CloudEvent"/> used to notify about the update of a projection
                /// </summary>
                public static readonly string Type = $"{TypePrefix}.v1";

            }

            /// <summary>
            /// Gets all versions of the projection updated event
            /// </summary>
            /// <returns>A new <see cref="IEnumerable{T}"/> containing all versions of the projection updated event</returns>
            public static IEnumerable<string> GetVersions()
            {
                yield return V1.Type;
            }

        }

        /// <summary>
        /// Exposes constants about the <see cref="CloudEvent"/> used to notify about the deletion of a projection
        /// </summary>
        public static class Deleted
        {

            /// <summary>
            /// Gets the prefix for the type of all versions of <see cref="CloudEvent"/>s used to notify about the deletion of a projection
            /// </summary>
            public static readonly string TypePrefix = $"{Projections.TypePrefix}.deleted";

            /// <summary>
            /// Exposes constants about the v1 of the <see cref="CloudEvent"/> used to notify about the deletion of a projection
            /// </summary>
            public static class V1
            {

                /// <summary>
                /// Gets the type of the v1 of the <see cref="CloudEvent"/> used to notify about the deletion of a projection
                /// </summary>
                public static readonly string Type = $"{TypePrefix}.v1";

            }

            /// <summary>
            /// Gets all versions of the projection deleted event
            /// </summary>
            /// <returns>A new <see cref="IEnumerable{T}"/> containing all versions of the projection deleted event</returns>
            public static IEnumerable<string> GetVersions()
            {
                yield return V1.Type;
            }

        }

        /// <summary>
        /// Gets all types of projection-related <see cref="CloudEvent"/>s
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> containing all types of projection-related <see cref="CloudEvent"/>s</returns>
        public static IEnumerable<string> GetTypes()
        {
            foreach (var version in Created.GetVersions()) yield return version;
            foreach (var version in Updated.GetVersions()) yield return version;
            foreach (var version in Deleted.GetVersions()) yield return version;
        }

    }

}
