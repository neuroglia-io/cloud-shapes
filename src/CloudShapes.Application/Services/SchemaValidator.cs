namespace CloudShapes.Application.Services;

/// <summary>
/// Represents the default implementation of the <see cref="ISchemaValidator"/> interface
/// </summary>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
public class SchemaValidator(IJsonSerializer jsonSerializer)
    : ISchemaValidator
{

    /// <summary>
    /// Gets the service used to serialize/deserialize data to/from JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <inheritdoc/>
    public virtual Task<IOperationResult> ValidateAsync(object input, JSchema schema, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(schema);
        var toValidate = (JObject)JsonSerializer.Convert(input, typeof(JObject))!;
        if (toValidate.IsValid(schema, errors: out var errors)) return Task.FromResult<IOperationResult>(new OperationResult((int)HttpStatusCode.OK));
        else return Task.FromResult<IOperationResult>(new OperationResult((int)HttpStatusCode.UnprocessableEntity, errors: errors.Select(e => e.ToError()).ToArray()));
    }

}
