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
