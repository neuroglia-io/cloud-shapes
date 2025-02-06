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
    /// <returns>A new <see cref="FilterDefinition{TDocument}"/></returns>
    public static FilterDefinition<T> BuildFilter<T>(this QueryOptions queryOptions)
    {
        var builder = Builders<T>.Filter;
        var filter = builder.Empty;
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
