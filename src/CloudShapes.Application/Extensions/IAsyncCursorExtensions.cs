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