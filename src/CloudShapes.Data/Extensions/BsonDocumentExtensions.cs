namespace CloudShapes.Data;

/// <summary>
/// Defines extensions for <see cref="BsonDocument"/>s
/// </summary>
public static partial class BsonDocumentExtensions
{

    /// <summary>
    /// Finds the <see cref="BsonValue"/> at the specified path
    /// </summary>
    /// <param name="document">The extended <see cref="BsonDocument"/></param>
    /// <param name="path">The JSON path of the value to find</param>
    /// <returns>The <see cref="BsonValue"/>, if any, at the specified path</returns>
    public static BsonValue? Find(this BsonDocument document, string path)
    {
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

    [GeneratedRegex(@"\[(\d+)\]$", RegexOptions.Compiled)]
    private static partial Regex ArrayIndexRegex();

}
