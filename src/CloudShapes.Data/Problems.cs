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

namespace CloudShapes.Data;

/// <summary>
/// Exposes constants about Cloud Shapes related problems
/// </summary>
public static class Problems
{

    /// <summary>
    /// Exposes constants about Cloud Shapes related problem types
    /// </summary>
    public static class Types
    {

        /// <summary>
        /// Gets the base uri for all Cloud Shapes problem types
        /// </summary>
        public static readonly Uri BaseUri = new("https://cloud-shapes.io/docs/problems/types");

        /// <summary>
        /// Gets the uri that describes the type of problem that occur when Cloud Shapes fails to create an index
        /// </summary>
        public static readonly Uri IndexCreationFailed = new(BaseUri, "/index-creation-failed");
        /// <summary>
        /// Gets the uri that describes the type of problems that occur when the specified key already exists
        /// </summary>
        public static readonly Uri KeyAlreadyExists = new(BaseUri, "/key-already-exists");
        /// <summary>
        /// Gets the uri that describes the type of problems that occur when Cloud Shapes failed to find the specified entity
        /// </summary>
        public static readonly Uri NotFound = new(BaseUri, "/not-found");
        /// <summary>
        /// Gets the uri that describes the type of problems that occur when attempting to apply a patch of an unsupported type
        /// </summary>
        public static readonly Uri UnsupportedPatchType = new(BaseUri, "/unsupported-patch-type");
        /// <summary>
        /// Gets the uri that describes the type of problems that occur when Cloud Shapes failed to validate data
        /// </summary>
        public static readonly Uri ValidationFailed = new(BaseUri, "/validation-failed");

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem types
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem types</returns>
        public static IEnumerable<Uri> AsEnumerable()
        {
            yield return IndexCreationFailed;
            yield return KeyAlreadyExists;
            yield return NotFound;
            yield return UnsupportedPatchType;
            yield return ValidationFailed;
        }

    }

    /// <summary>
    /// Exposes constants about Cloud Shapes related problem titles
    /// </summary>
    public static class Titles
    {

        /// <summary>
        /// Gets the title of the problem that occurs when Cloud Shapes fails to create an index
        /// </summary>
        public const string IndexCreationFailed = "Index Creation Failed";
        /// <summary>
        /// Gets the title of the problem that occurs when the specified key already exists
        /// </summary>
        public const string KeyAlreadyExists = "Key Already Exists";
        /// <summary>
        /// Gets the title of the problem that occurs when Cloud Shapes failed to find the specified entity
        /// </summary>
        public const string NotFound = "Not Found";
        /// <summary>
        /// Gets the title of the problem that occurs when attempting to apply a patch of an unsupported type
        /// </summary>
        public const string UnsupportedPatchType = "Unsupported Patch Type";
        /// <summary>
        /// Gets the title of the problem that occurs when Cloud Shapes failed to validate data
        /// </summary>
        public const string ValidationFailed = "Validation Failed";

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem titles
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem titles</returns>
        public static IEnumerable<string> AsEnumerable()
        {
            yield return IndexCreationFailed;
            yield return KeyAlreadyExists;
            yield return NotFound;
            yield return UnsupportedPatchType;
            yield return ValidationFailed;
        }

    }

    /// <summary>
    /// Exposes constants about Cloud Shapes related problem statuses
    /// </summary>
    public static class Statuses
    {

        /// <summary>
        /// Gets the status for problems that describe the failure to find a specific entity
        /// </summary>
        public const int NotFound = (int)HttpStatusCode.NotFound;
        /// <summary>
        /// Gets the status for problems that describe an unprocessable operation
        /// </summary>
        public const int Unprocessable = (int)HttpStatusCode.UnprocessableContent;
        /// <summary>
        /// Gets the status for problems that describe validation failures
        /// </summary>
        public const int ValidationFailed = (int)HttpStatusCode.UnprocessableContent;

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem statuses
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem statuses</returns>
        public static IEnumerable<int> AsEnumerable()
        {
            yield return NotFound;
            yield return Unprocessable;
            yield return ValidationFailed;
        }

    }

    /// <summary>
    /// Exposes constants about Cloud Shapes related problem details
    /// </summary>
    public static class Details
    {

        /// <summary>
        /// Gets the details template of a problem due to the existence of a projection type with the specified key
        /// </summary>
        public const string ProjectionTypeKeyAlreadyExists = "A projection type with the specified name '{name}' already exists";
        /// <summary>
        /// Gets the details template of a problem due to the existence of a projection with the specified key
        /// </summary>
        public const string ProjectionKeyAlreadyExists = "A projection of type '{type}' with the specified id '{id}' already exists";
        /// <summary>
        /// Gets the details template of a problem due to the failure to find the specified projection type
        /// </summary>
        public const string ProjectionTypeNotFound = "Failed to find a projection type with name '{name}'";
        /// <summary>
        /// Gets the details template of a problem due to the failure to find the specified projection
        /// </summary>
        public const string ProjectionNotFound = "Failed to find a projection of type '{type}' with the specified id '{id}'";
        /// <summary>
        /// Gets the details template of a problem due to a projection validation failure
        /// </summary>
        public const string ProjectionValidationFailed = "Failed to validate a projection of type '{type}':\r\n{errors}";
        /// <summary>
        /// Gets the details template of a problem due to an unsupported patch type
        /// </summary>
        public const string UnsupportedPatchType = "Failed to find an handler for the specified patch type '{type}'";
    }

}
