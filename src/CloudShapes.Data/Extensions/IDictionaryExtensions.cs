﻿// Copyright © 2025-Present The Cloud Shapes Authors
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

using System.Collections;

namespace CloudShapes.Data;

/// <summary>
/// Defines extensions for <see cref="IDictionary{TKey, TValue}"/> instances
/// </summary>
public static partial class IDictionaryExtensions
{

    /// <summary>
    /// Finds the value at the specified path
    /// </summary>
    /// <param name="map">The extended <see cref="IDictionary{TKey, TValue}"/></param>
    /// <param name="path">The JSON path of the value to find</param>
    /// <returns>The value, if any, at the specified path</returns>
    public static object? Find(this IDictionary<string, object> map, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (string.IsNullOrEmpty(path)) return null;
        var jsonPath = path.StartsWith("$.") ? path[2..] : path;
        var segments = jsonPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var value = map as object;
        var regex = ArrayIndexRegex();
        foreach (var segment in segments)
        {
            if (value is IDictionary<string, object> subMap)
            {
                if (regex.IsMatch(segment))
                {
                    var arrayMatch = regex.Match(segment);
                    var arrayKey = segment[..segment.IndexOf('[')];
                    var arrayIndex = int.Parse(arrayMatch.Groups[1].Value);
                    if (!subMap.ContainsKey(arrayKey)) return null;
                    if (subMap[arrayKey] is not IEnumerable enumerable) return null;
                    var array = enumerable.OfType<object>();
                    if (arrayIndex >= array.Count()) return null;
                    value = array.ElementAt(arrayIndex);
                }
                else
                {
                    if (!subMap.ContainsKey(segment)) return null;
                    value = subMap[segment];
                }
            }
            else if (value is BsonArray array)
            {
                if (!regex.IsMatch(segment)) return null;
                var arrayMatch = regex.Match(segment);
                var arrayIndex = int.Parse(arrayMatch.Groups[1].Value);
                if (arrayIndex >= array.Count) return null;
                value = array[arrayIndex];
            }
            else return null;
        }
        return value;
    }

    [GeneratedRegex(@"\[(\d+)\]$", RegexOptions.Compiled)]
    private static partial Regex ArrayIndexRegex();

}
