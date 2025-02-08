namespace CloudShapes.Application.Services;

/// <summary>
/// Defines the fundamentals of a service used to validate schemas
/// </summary>
public interface ISchemaValidator
{

    /// <summary>
    /// Validates the input against the specified <see cref="JSchema"/>
    /// </summary>
    /// <param name="input">The input to validate</param>
    /// <param name="schema">The <see cref="JSchema"/> to validate the input against</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IOperationResult"/> that describes the result of the operation</returns>
    Task<IOperationResult> ValidateAsync(object input, JsonSchema schema, CancellationToken cancellationToken = default);

}
