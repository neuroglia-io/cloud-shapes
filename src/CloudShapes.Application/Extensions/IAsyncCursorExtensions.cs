namespace CloudShapes.Application;

/// <summary>
/// Defines extensions for <see cref="IAsyncCursor{TDocument}"/> instances
/// </summary>
public static class IAsyncCursorExtensions
{

    /// <summary>
    /// Converts the specified <see cref="IAsyncCursor{TDocument}"/> into a new <see cref="IAsyncEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of data to enumerate</typeparam>
    /// <param name="cursor">The extended <see cref="IAsyncCursor{TDocument}"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new <see cref="IAsyncEnumerable{T}"/></returns>
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IAsyncCursor<T> cursor, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false)) foreach (var doc in cursor.Current) yield return doc;
        cursor.Dispose();
    }

}