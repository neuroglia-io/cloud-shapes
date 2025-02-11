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

    /// <summary>
    /// Recursively finds all object properties within a <see cref="JsonSchema"/>
    /// </summary>
    /// <param name="schema">The extended <see cref="JsonSchema"/></param>
    /// <returns>A list of the fully qualified names of all object properties found</returns>
    public static List<string> GetObjectProperties(this JsonSchema schema)
    {
        var results = new List<string>();
        GetObjectPropertiesRecursively(schema, "", results);
        return results;
    }

    static void GetObjectPropertiesRecursively(JsonSchema? schema, string prefix, List<string> results)
    {
        var properties = schema?.GetProperties();
        if (properties == null) return;
        foreach (var property in properties)
        {
            var propertyName = string.IsNullOrEmpty(prefix) ? property.Key : $"{prefix}.{property.Key}";
            switch (property.Value.GetJsonType())
            {
                case SchemaValueType.Array:
                    GetPrimitivePropertiesRecursively(property.Value.GetItems(), propertyName, results);
                    break;
                case SchemaValueType.Object:
                    results.Add(propertyName);
                    GetPrimitivePropertiesRecursively(property.Value, propertyName, results);
                    break;
            }
        }
    }

    /// <summary>
    /// Recursively finds all array properties within a <see cref="JsonSchema"/>
    /// </summary>
    /// <param name="schema">The extended <see cref="JsonSchema"/></param>
    /// <returns>A list of the fully qualified names of all array properties found</returns>
    public static List<string> GetArrayProperties(this JsonSchema schema)
    {
        var results = new List<string>();
        GetArrayPropertiesRecursively(schema, "", results);
        return results;
    }

    static void GetArrayPropertiesRecursively(JsonSchema? schema, string prefix, List<string> results)
    {
        var properties = schema?.GetProperties();
        if (properties == null) return;
        foreach (var property in properties)
        {
            var propertyName = string.IsNullOrEmpty(prefix) ? property.Key : $"{prefix}.{property.Key}";
            switch (property.Value.GetJsonType())
            {
                case SchemaValueType.Array:
                    results.Add(propertyName);
                    GetPrimitivePropertiesRecursively(property.Value.GetItems(), propertyName, results);
                    break;
                case SchemaValueType.Object:
                    GetPrimitivePropertiesRecursively(property.Value, propertyName, results);
                    break;
            }
        }
    }

}