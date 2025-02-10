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
    public virtual Task<IOperationResult> ValidateAsync(object input, JsonSchema schema, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(schema);
        var node = JsonSerializer.SerializeToNode(input);
        var evaluationOptions = new EvaluationOptions()
        {
            OutputFormat = OutputFormat.List
        };
        var evaluationResults = schema.Evaluate(node, evaluationOptions);
        if (evaluationResults.IsValid) return Task.FromResult<IOperationResult>(new OperationResult((int)HttpStatusCode.OK));
        else return Task.FromResult<IOperationResult>(new OperationResult((int)HttpStatusCode.UnprocessableEntity, errors: evaluationResults.Details.Where(d => d.Errors != null).SelectMany(d => d.Errors!).Select(e => new Error(new("io.cloud-shapes.errors.invalid"), "Invalid", (int)HttpStatusCode.UnprocessableEntity, e.Value, new(e.Key, UriKind.Relative))).ToArray()!));
    }

}
