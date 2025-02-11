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

using MongoDB.Bson.IO;

namespace CloudShapes.Data;

/// <summary>
/// Defines extensions for <see cref="BsonDocument"/>s
/// </summary>
public static partial class BsonDocumentExtensions
{

    /// <summary>
    /// Gets the <see cref="BsonDocument"/>'s id, if any
    /// </summary>
    /// <param name="document">The <see cref="BsonDocument"/> to get the id of, if any</param>
    /// <returns>The <see cref="BsonDocument"/>'s id, if any</returns>
    public static string? GetId(this BsonDocument document)
    {
        if (document.TryGetValue("_id", out var id)) return id.ToString();
        else return null;
    }

    /// <summary>
    /// Finds the <see cref="BsonValue"/> at the specified path
    /// </summary>
    /// <param name="document">The extended <see cref="BsonDocument"/></param>
    /// <param name="path">The JSON path of the value to find</param>
    /// <returns>The <see cref="BsonValue"/>, if any, at the specified path</returns>
    public static BsonValue? Find(this BsonDocument document, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (string.IsNullOrEmpty(path)) return null;
        var jsonPath = path.StartsWith("$.") ? path[2..] : path;
        var segments = jsonPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var value = document as BsonValue;
        var regex = ArrayIndexRegex();
        foreach (var segment in segments)
        {
            if (value is BsonDocument subDocument)
            {
                if (regex.IsMatch(segment))
                {
                    var arrayMatch = regex.Match(segment);
                    var arrayKey = segment[..segment.IndexOf('[')];
                    var arrayIndex = int.Parse(arrayMatch.Groups[1].Value);
                    if (!subDocument.Contains(arrayKey)) return null;
                    if (!subDocument[arrayKey].IsBsonArray) return null;
                    var array = subDocument[arrayKey].AsBsonArray;
                    if (arrayIndex >= array.Count) return null;
                    value = array[arrayIndex];
                }
                else
                {
                    if (!subDocument.Contains(segment)) return null;
                    value = subDocument[segment];
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

    /// <summary>
    /// Replaces a value in a BsonDocument using a dot-separated path.
    /// </summary>
    /// <param name="document">The BsonDocument to modify</param>
    /// <param name="path">The path of the value to replace</param>
    /// <param name="value">The new value to set</param>
    public static void Replace(this BsonDocument document, string path, BsonValue value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var segments = path.Split('.');
        ReplaceRecursive(document, segments, 0, value);
    }

    static void ReplaceRecursive(BsonValue current, string[] segments, int index, BsonValue newValue)
    {
        if (index >= segments.Length) return;
        var segment = segments[index];
        var match = ArrayIndexRegex().Match(segment);
        if (match.Success)
        {
            var arrayKey = segment[..segment.IndexOf('[')];
            var arrayIndex = int.Parse(match.Groups[1].Value);
            if (current is BsonDocument document)
            {
                if (!document.Contains(arrayKey) || !document[arrayKey].IsBsonArray) document[arrayKey] = new BsonArray();
                var array = document[arrayKey].AsBsonArray;
                while (array.Count <= arrayIndex) array.Add(new BsonDocument());
                if (index == segments.Length - 1) array[arrayIndex] = newValue;
                else ReplaceRecursive(array[arrayIndex], segments, index + 1, newValue);
            }
        }
        else if (current is BsonDocument doc)
        {
            if (index == segments.Length - 1)  doc[segment] = newValue;
            else
            {
                if (!doc.Contains(segment) || !doc[segment].IsBsonDocument) doc[segment] = new BsonDocument();
                ReplaceRecursive(doc[segment], segments, index + 1, newValue);
            }
        }
    }

    /// <summary>
    /// Replaces a document within a BsonArray at the specified path, matching by GetId().
    /// If a matching document is found, it is replaced with the new document.
    /// If no match is found, no changes are made.
    /// </summary>
    /// <param name="document">The BsonDocument containing the array.</param>
    /// <param name="path">The dot-separated path to the BsonArray.</param>
    /// <param name="value">The new document to replace the existing one.</param>
    public static void ReplaceInArray(this BsonDocument document, string path, BsonDocument value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(value);
        var id = value.GetId() ?? throw new InvalidOperationException("Replacement document must have a valid ID.");
        var array = document.Find(path) as BsonArray;
        if (array == null) return;
        for (var i = 0; i < array.Count; i++)
        {
            var item = array[i].AsBsonDocument;
            if (item.GetId()?.Equals(id) == true)
            {
                array[i] = value;
                return;
            }
        }
    }

    /// <summary>
    /// Adds the specified value to the array at the given path. If the path does not exist or is not an array, it creates one.
    /// </summary>
    /// <param name="document">The BsonDocument to modify.</param>
    /// <param name="path">The dot-separated path to the array property.</param>
    /// <param name="value">The value to add to the array.</param>
    public static void AddTo(this BsonDocument document, string path, BsonValue value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(value);
        if (string.IsNullOrEmpty(path)) return;
        var segments = path.Split('.');
        AddToRecursive(document, segments, 0, value);
    }

    static void AddToRecursive(BsonValue current, string[] segments, int index, BsonValue newValue)
    {
        if (index >= segments.Length) return;
        var segment = segments[index];
        var match = ArrayIndexRegex().Match(segment);
        if (match.Success)
        {
            var arrayKey = segment[..segment.IndexOf('[')];
            var arrayIndex = int.Parse(match.Groups[1].Value);
            if (current is BsonDocument document)
            {
                if (!document.Contains(arrayKey) || !document[arrayKey].IsBsonArray) document[arrayKey] = new BsonArray();
                var array = document[arrayKey].AsBsonArray;
                if (array.Count <= arrayIndex) return;
                AddToRecursive(array[arrayIndex], segments, index + 1, newValue);
            }
        }
        else if (current is BsonDocument doc)
        {
            if (index == segments.Length - 1)
            {
                if (!doc.Contains(segment) || !doc[segment].IsBsonArray) doc[segment] = new BsonArray();
                doc[segment].AsBsonArray.Add(newValue);
            }
            else
            {
                if (!doc.Contains(segment) || !doc[segment].IsBsonDocument) doc[segment] = new BsonDocument();
                AddToRecursive(doc[segment], segments, index + 1, newValue);
            }
        }
    }

    /// <summary>
    /// Removes the property at the specified path from the BsonDocument.
    /// </summary>
    /// <param name="document">The BsonDocument to modify.</param>
    /// <param name="path">The dot-separated path to the property to remove.</param>
    public static void RemoveAt(this BsonDocument document, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var segments = path.Split('.');
        RemoveAtRecursive(document, segments, 0);
    }

    static void RemoveAtRecursive(BsonValue current, string[] segments, int index)
    {
        if (index >= segments.Length || current is not BsonDocument doc) return;
        var segment = segments[index];
        if (index == segments.Length - 1) doc.Remove(segment);
        else if (doc.Contains(segment) && doc[segment].IsBsonDocument) RemoveAtRecursive(doc[segment], segments, index + 1);
    }

    /// <summary>
    /// Removes the document with the specified id from the array at the given path.
    /// </summary>
    /// <param name="document">The BsonDocument containing the array.</param>
    /// <param name="path">The dot-separated path to the BsonArray.</param>
    /// <param name="id">The id of the document to remove.</param>
    public static void RemoveFrom(this BsonDocument document, string path, string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var array = document.Find(path) as BsonArray;
        if (array == null) return;
        for (var i = 0; i < array.Count; i++)
        {
            var item = array[i].AsBsonDocument;
            if (item.GetId()?.Equals(id) == true)
            {
                array.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// Inserts <see cref="DocumentMetadata"/> into the <see cref="BsonDocument"/>
    /// </summary>
    /// <param name="document">The extended <see cref="BsonDocument"/></param>
    /// <param name="metadata">The <see cref="DocumentMetadata"/> to insert</param>
    /// <returns>The updated <see cref="BsonDocument"/></returns>
    public static BsonDocument InsertMetadata(this BsonDocument document, DocumentMetadata metadata)
    {
        var newDocument = new BsonDocument();
        if (document.Contains("_id")) newDocument["_id"] = document["_id"];
        newDocument[DocumentMetadata.PropertyName] = metadata.ToBsonDocument();
        foreach (var element in document.Elements)
        {
            if (element.Name == "_id" || element.Name == DocumentMetadata.PropertyName) continue;
            newDocument[element.Name] = element.Value;
        }
        return newDocument;
    }

    /// <summary>
    /// Gets the projection's state
    /// </summary>
    /// <param name="document">The <see cref="BsonDocument"/> to extract the projection's state from</param>
    /// <param name="serializer">The <see cref="IJsonSerializer"/> used to deserialize the projection's state</param>
    /// <returns>The projection's state</returns>
    public static object GetState(this BsonDocument document, IJsonSerializer serializer)
    {
        var state = document.DeepClone().AsBsonDocument;
        state.Remove("_id");
        state.Remove(DocumentMetadata.PropertyName);
        var json = state.ToJson(new() { OutputMode = JsonOutputMode.RelaxedExtendedJson });
        return serializer.Deserialize<object>(json)!;
    }

    [GeneratedRegex(@"\[(\d+)\]$", RegexOptions.Compiled)]
    private static partial Regex ArrayIndexRegex();

}
