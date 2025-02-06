namespace CloudShapes.Application;

/// <summary>
/// Defines extensions for <see cref="ValidationError"/>s
/// </summary>
public static class ValidationErrorExtensions
{

    /// <summary>
    /// Converts the <see cref="ValidationError"/> into a new <see cref="Error"/>
    /// </summary>
    /// <param name="error">The <see cref="ValidationError"/> to convert</param>
    /// <returns>A new <see cref="Error"/></returns>
    public static Error ToError(this ValidationError error)
    {
        return new()
        {
            Type = new("io.cloud-shapes.errors.invalid"),
            Title = "Invalid",
            Status = (int)HttpStatusCode.UnprocessableEntity,
            Detail = error.Message,
            Instance = new(error.Path.Replace('.', '/'), UriKind.Relative)
        };
    }

}
