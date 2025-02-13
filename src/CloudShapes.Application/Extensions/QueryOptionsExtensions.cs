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

using CloudShapes.Integration.Models;

namespace CloudShapes.Application;

/// <summary>
/// Defines extensions for <see cref="QueryOptions"/>
/// </summary>
public static class QueryOptionsExtensions
{

    /// <summary>
    /// Builds a <see cref="FilterDefinition{TDocument}"/> based on the provided query options.
    /// </summary>
    /// <typeparam name="T">The type of data to build the <see cref="FilterDefinition{TDocument}"/> for</typeparam>
    /// <param name="queryOptions">The extended <see cref="QueryOptions"/></param>
    /// <param name="type">The type of projection to build the <see cref="FilterDefinition{TDocument}"/> for</param>
    /// <returns>A new <see cref="FilterDefinition{TDocument}"/></returns>
    public static FilterDefinition<T> BuildFilter<T>(this QueryOptions queryOptions, ProjectionType? type = null)
    {
        var builder = Builders<T>.Filter;
        var filter = builder.Empty;
        if (!string.IsNullOrWhiteSpace(queryOptions.Search))
        {
            var searchRegex = new BsonRegularExpression(Regex.Escape(queryOptions.Search), "i");
            if (typeof(T) == typeof(BsonDocument))
            {
                if (type == null)
                {
                    filter &= Builders<T>.Filter.Text(queryOptions.Search, new TextSearchOptions() { CaseSensitive = false, DiacriticSensitive = false });
                }
                else
                {
                    var properties = new List<string>()
                    {
                        { "_id" },
                        { "_metadata.createdAt" },
                        { "_metadata.lastModified" },
                        { "_metadata.version" }
                    };
                    properties.AddRange(type.Schema.GetPrimitiveProperties());
                    var orFilters = new List<FilterDefinition<T>>();
                    foreach (var property in properties) orFilters.Add(builder.Regex(property, searchRegex));
                    filter &= builder.Or(orFilters);
                }
            }
            else
            {
                var orFilters = new List<FilterDefinition<T>>();
                var properties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(object));
                foreach (var property in properties) orFilters.Add(builder.Regex(property.Name, searchRegex));
                filter &= builder.Or(orFilters);
            }
        }
        if (queryOptions.Filters != null)
        {
            foreach (var entry in queryOptions.Filters)
            {
                var fieldName = entry.Key.ToLowerInvariant();
                if (typeof(T) == typeof(ProjectionType) && fieldName.Equals("name", StringComparison.OrdinalIgnoreCase)) fieldName = "_id";
                var fieldValue = entry.Value;
                if (!string.IsNullOrWhiteSpace(fieldValue)) filter &= builder.Regex(fieldName, new BsonRegularExpression(Regex.Escape(fieldValue), "i"));
            }
        }
        return filter;
    }

}
