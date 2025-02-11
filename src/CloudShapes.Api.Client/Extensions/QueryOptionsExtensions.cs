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

using CloudShapes.Integration;

namespace CloudShapes.Api.Client;

/// <summary>
/// Defines extensions for <see cref="QueryOptions"/>
/// </summary>
public static class QueryOptionsExtensions
{

    /// <summary>
    /// Converts <see cref="QueryOptions"/> into a URL query string
    /// </summary>
    /// <param name="options">The <see cref="QueryOptions"/>  to convert</param>
    /// <returns>A query string representation of the <see cref="QueryOptions"/> </returns>
    public static string ToQueryString(this QueryOptions options)
    {
        if (options == null) return string.Empty;
        var queryParams = new List<string>();
        if (options.Limit.HasValue) queryParams.Add($"limit={options.Limit.Value}");
        if (options.Skip.HasValue) queryParams.Add($"skip={options.Skip.Value}");
        if (!string.IsNullOrWhiteSpace(options.Search)) queryParams.Add($"search={options.Search}");
        if (!string.IsNullOrWhiteSpace(options.OrderBy))
        {
            queryParams.Add($"search={options.OrderBy}");
            if (options.Descending) queryParams.Add("descending=true");
        }
        if (options.Filters != null && options.Filters.Count != 0)
        {
            foreach (var filter in options.Filters)
            {
                var encodedKey = WebUtility.UrlEncode(filter.Key);
                var encodedValue = WebUtility.UrlEncode(filter.Value);
                queryParams.Add($"filters[{encodedKey}]={encodedValue}");
            }
        }
        return queryParams.Count != 0 ? string.Join("&", queryParams) : string.Empty;
    }

}
