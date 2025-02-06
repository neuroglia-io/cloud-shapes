using System.Net;

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
        if (options.Filters != null && options.Filters.Count != 0)
        {
            foreach (var filter in options.Filters)
            {
                var encodedKey = WebUtility.UrlEncode(filter.Key);
                var encodedValue = WebUtility.UrlEncode(filter.Value);
                queryParams.Add($"filters[{encodedKey}]={encodedValue}");
            }
        }
        return queryParams.Count != 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }

}
