namespace CloudShapes.Application;

/// <summary>
/// Defines extensions for <see cref="JsonSchema"/>s
/// </summary>
public static class JsonSchemaExtensions
{

    /// <summary>
    /// Recursively finds all primitive properties within a <see cref="JsonSchema"/>
    /// </summary>
    /// <param name="schema">The extended <see cref="JsonSchema"/></param>
    /// <returns>A list of the fully qualified names of all primitive properties found</returns>
    public static List<string> GetPrimitiveProperties(this JsonSchema schema)
    {
        var results = new List<string>();
        GetPrimitivePropertiesRecursively(schema, "", results);
        return results;
    }

    static void GetPrimitivePropertiesRecursively(JsonSchema? schema, string prefix, List<string> results)
    {
        var properties = schema?.GetProperties();
        if (properties == null) return;
        foreach (var property in properties)
        {
            var propertyName = string.IsNullOrEmpty(prefix) ? property.Key : $"{prefix}.{property.Key}";
            switch (property.Value.GetJsonType())
            {
                case SchemaValueType.Boolean or SchemaValueType.Integer or SchemaValueType.Number or SchemaValueType.String:
                    results.Add(propertyName);
                    break;
                case SchemaValueType.Array:
                    GetPrimitivePropertiesRecursively(property.Value.GetItems(), propertyName, results);
                    break;
                case SchemaValueType.Object:
                    GetPrimitivePropertiesRecursively(property.Value, propertyName, results);
                    break;
            }
        }
    }

}