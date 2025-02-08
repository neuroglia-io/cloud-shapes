namespace CloudShapes.Application.Services;

/// <summary>
/// Represents an empty <see cref="IAsyncCursor{TDocument}"/> implementation
/// </summary>
/// <typeparam name="T">The type enumerated by the <see cref="IAsyncCursor{TDocument}"/></typeparam>
public class EmptyAsyncCursor<T> 
    : IAsyncCursor<T>
{

    /// <inheritdoc/>
    public IEnumerable<T> Current => Enumerable.Empty<T>();

    /// <inheritdoc/>
    public bool MoveNext(CancellationToken cancellationToken = default) => false;

    /// <inheritdoc/>
    public Task<bool> MoveNextAsync(CancellationToken cancellationToken) => Task.FromResult(false);

    /// <inheritdoc/>
    public void Dispose() { }

}