namespace CloudShapes.Dashboard.Components.MonacoEditorStateManagement;

/// <summary>
/// Represents the state of a <see cref="MonacoEditor"/>
/// </summary>
public record MonacoEditorState
{

    /// <summary>
    /// Gets/sets the text representation of the referenced document
    /// </summary>
    public string DocumentText { get; set; } = string.Empty;

    /// <summary>
    /// Gets/sets a boolean indicating the editor is read-only
    /// </summary>
    public bool IsReadOnly { get; set; } = false;

    /// <summary>
    /// Gets/sets the document's model name, if any
    /// </summary>
    public string ModelName { get; set; } = string.Empty;

}
