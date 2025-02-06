namespace CloudShapes.Dashboard.Services;

/// <summary>
/// The service used to build a bridge with the localStorage
/// </summary>
/// <param name="jsRuntime">The service used to interop with JS</param>
public class LocalStorage(IJSRuntime jsRuntime)
    : ILocalStorage
{

    /// <inheritdoc/>
    public ValueTask<string?> GetItemAsync(string key) => jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);

    /// <inheritdoc/>
    public ValueTask SetItemAsync(string key, string value) =>  jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);

    /// <inheritdoc/>
    public ValueTask RemoveItemAsync(string key) => jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);

    /// <inheritdoc/>
    public ValueTask ClearAsync() => jsRuntime.InvokeVoidAsync("localStorage.clear");

    /// <inheritdoc/>
    public ValueTask<int> LengthAsync() => jsRuntime.InvokeAsync<int>("eval", "localStorage.length");

    /// <inheritdoc/>
    public ValueTask<string?> KeyAsync(int index) => jsRuntime.InvokeAsync<string?>("localStorage.key", index);

    /// <inheritdoc/>
    public ValueTask<IEnumerable<string>> KeysAsync() => jsRuntime.InvokeAsync<IEnumerable<string>>("eval", "Object.keys(localStorage)");

}
