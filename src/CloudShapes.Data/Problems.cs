using System.Net;

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
        /// Gets an <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem types
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem types</returns>
        public static IEnumerable<Uri> AsEnumerable()
        {
            yield return IndexCreationFailed;
            yield return KeyAlreadyExists;
            yield return NotFound;
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
        /// Gets an <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem titles
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem titles</returns>
        public static IEnumerable<string> AsEnumerable()
        {
            yield return IndexCreationFailed;
            yield return KeyAlreadyExists;
            yield return NotFound;
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
        /// Gets an <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem statuses
        /// </summary>
        /// <returns>A new <see cref="IEnumerable{T}"/> that contains all Cloud Shapes problem statuses</returns>
        public static IEnumerable<int> AsEnumerable()
        {
            yield return NotFound;
            yield return Unprocessable;
        }

    }

}
