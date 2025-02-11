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

using BlazorMonaco.Editor;

namespace CloudShapes.Dashboard.Components.MonacoEditorStateManagement;

/// <summary>
/// Represents the <see cref="ComponentStore{TState}" /> of a <see cref="MonacoEditor"/>
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="apiClient">The service used interact with CloudShapes API</param>
/// <param name="jsRuntime">The service used from JS interop</param>
/// <param name="monacoEditorHelper">The service used ease Monaco Editor interactions</param>
/// <param name="jsonSerializer">The service used to serialize and deserialize JSON</param>
/// <param name="yamlSerializer">The service used to serialize and deserialize YAML</param>
/// <param name="toastService">The service used display toast messages</param>
public class MonacoEditorStore(ILogger<MonacoEditorStore> logger, ICloudShapesApiClient apiClient, IJSRuntime jsRuntime, IMonacoEditorHelper monacoEditorHelper, IJsonSerializer jsonSerializer, IYamlSerializer yamlSerializer, ToastService toastService)
    : ComponentStore<MonacoEditorState>(new())
{

    bool _disposed;
    TextModel? _textModel;
    string _textModelUri = monacoEditorHelper.GetResourceUri();
    bool _hasTextEditorInitialized = false;

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger<MonacoEditorStore> Logger { get; } = logger;

    /// <summary>
    /// Gets the service used to interact with the CloudShapes API
    /// </summary>
    protected ICloudShapesApiClient ApiClient { get; } = apiClient;

    /// <summary>
    /// Gets the service used for JS interop
    /// </summary>
    protected IJSRuntime JSRuntime { get; } = jsRuntime;

    /// <summary>
    /// Gets the service used ease Monaco Editor interactions
    /// </summary>
    protected IMonacoEditorHelper MonacoEditorHelper { get; } = monacoEditorHelper;

    /// <summary>
    /// Gets the service used to serialize and deserialize JSON
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; } = jsonSerializer;

    /// <summary>
    /// Gets the service used to serialize and deserialize YAML
    /// </summary>
    protected IYamlSerializer YamlSerializer { get; } = yamlSerializer;

    /// <summary>
    /// Gets the service used display toast messages
    /// </summary>
    protected ToastService ToastService { get; } = toastService;

    /// <summary>
    /// The <see cref="BlazorMonaco.Editor.StandaloneEditorConstructionOptions"/> provider function
    /// </summary>
    public Func<StandaloneCodeEditor, StandaloneEditorConstructionOptions> StandaloneEditorConstructionOptions = monacoEditorHelper.GetStandaloneEditorConstructionOptions(string.Empty, false, monacoEditorHelper.PreferredLanguage);

    /// <summary>
    /// The <see cref="StandaloneCodeEditor"/> reference
    /// </summary>
    public StandaloneCodeEditor? TextEditor { get; set; }

    #region Selectors
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="MonacoEditorState.IsReadOnly"/> changes
    /// </summary>
    public IObservable<bool> IsReadOnly => this.Select(state => state.IsReadOnly).DistinctUntilChanged();
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="MonacoEditorState.DocumentText"/> changes
    /// </summary>
    public IObservable<string> DocumentText => this.Select(state => state.DocumentText).DistinctUntilChanged();
    #endregion

    #region Setters
    /// <summary>
    /// Sets the state's <see cref="MonacoEditorState.IsReadOnly"/>
    /// </summary>
    /// <param name="isReadOnly">The new value</param>
    public void SetIsReadOnly(bool isReadOnly)
    {
        var current = this.Get(state => state.IsReadOnly);
        if (current != isReadOnly)
        {
            this.Reduce(state => state with { 
                IsReadOnly = isReadOnly 
            });
        }
    }

    /// <summary>
    /// Sets the state's <see cref="MonacoEditorState.DocumentText"/> based on the provided object document
    /// </summary>
    /// <param name="document">The object to build the new <see cref="MonacoEditorState.DocumentText"/> with</param>
    public void SetDocument(object? document)
    {
        string documentText = string.Empty;
        if (document != null)
        {
            try
            {
                documentText = this.MonacoEditorHelper.PreferredLanguage == PreferredLanguage.JSON ?
                    this.JsonSerializer.SerializeToText(document) :
                    this.YamlSerializer.SerializeToText(document);
            }
            catch /*(Exception ex)*/
            {
                // todo: handle ex
            }
        }
        this.Reduce(state => state with
        {
            DocumentText = documentText
        });
    }

    /// <summary>
    /// Sets the state's <see cref="MonacoEditorState.DocumentText"/> based on the provided JSON document
    /// </summary>
    /// <param name="documentJson">The JSON document to build the new <see cref="MonacoEditorState.DocumentText"/> with</param>
    public void SetDocumentJson(string? documentJson)
    {
        var documentText = documentJson ?? string.Empty;
        if (this.MonacoEditorHelper.PreferredLanguage == PreferredLanguage.YAML)
        {
            try
            {
                documentText = this.YamlSerializer.ConvertFromJson(documentText);
            }
            catch /*(Exception ex)*/
            {
                //todo: handle ex
            }
        }
        this.Reduce(state => state with
        {
            DocumentText = documentText
        });
    }

    /// <summary>
    /// Sets the state's <see cref="MonacoEditorState.DocumentText"/> based on the provided YAML document
    /// </summary>
    /// <param name="documentYaml">The YAML document to build the new <see cref="MonacoEditorState.DocumentText"/> with</param>
    public void SetDocumentYaml(string? documentYaml)
    {
        var documentText = documentYaml ?? string.Empty;
        if (this.MonacoEditorHelper.PreferredLanguage == PreferredLanguage.JSON)
        {
            try
            {
                documentText = this.YamlSerializer.ConvertToJson(documentText);
            }
            catch /*(Exception ex)*/
            {
                //todo: handle ex
            }
        }
        this.Reduce(state => state with
        {
            DocumentText = documentText
        });
    }
    /// <summary>
    /// Sets the state's <see cref="MonacoEditorState.ModelName"/>
    /// </summary>
    /// <param name="modelName">The new value</param>
    public void SetTexModelName(string modelName)
    {
        var current = this.Get(state => state.ModelName);
        if (current != modelName)
        {
            this.Reduce(state => state with
            {
                ModelName = modelName
            });
            this._textModelUri = !string.IsNullOrEmpty(modelName) ? this.MonacoEditorHelper.GetResourceUri(modelName) : this.MonacoEditorHelper.GetResourceUri();
        }
    }
    #endregion

    #region Actions
    /// <summary>
    /// Handles changed of the text editor's language
    /// </summary>
    /// <param name="_"></param>
    /// <returns></returns>
    public async Task ToggleTextBasedEditorLanguageAsync(string _)
    {
        if (this.TextEditor == null || !this._hasTextEditorInitialized)  return;
        var language = this.MonacoEditorHelper.PreferredLanguage;
        try
        {
            var document = await this.TextEditor.GetValue();
            if (document == null) return;
            document = language == PreferredLanguage.YAML ?
                this.YamlSerializer.ConvertFromJson(document) :
                this.YamlSerializer.ConvertToJson(document);
            this.Reduce(state => state with
            {
                DocumentText = document
            });
            await this.InitializeTextBasedEditorAsync();
        }
        catch (Exception ex)
        {
            this.Logger.LogError("Unable to set text editor value: {exception}", ex.ToString());
            await this.MonacoEditorHelper.ChangePreferredLanguageAsync(language == PreferredLanguage.YAML ? PreferredLanguage.JSON : PreferredLanguage.YAML);
        }
    }

    /// <summary>
    /// Handles initialization of the text editor
    /// </summary>
    /// <returns></returns>
    public async Task OnTextBasedEditorInitAsync()
    {
        this._hasTextEditorInitialized = true;
        await this.InitializeTextBasedEditorAsync();
    }

    /// <summary>
    /// Initializes the text editor
    /// </summary>
    /// <returns></returns>
    public async Task InitializeTextBasedEditorAsync()
    {
        if (this.TextEditor == null || !this._hasTextEditorInitialized) return;
        await this.SetTextBasedEditorLanguageAsync();
        await this.SetTextEditorValueAsync();
    }

    /// <summary>
    /// Sets the language of the text editor
    /// </summary>
    /// <returns></returns>
    public async Task SetTextBasedEditorLanguageAsync()
    {
        try
        {
            var language = this.MonacoEditorHelper.PreferredLanguage;
            if (this.TextEditor != null && this._hasTextEditorInitialized)
            {
                this._textModel = await Global.GetModel(this.JSRuntime, this._textModelUri);
                this._textModel ??= await Global.CreateModel(this.JSRuntime, "", language, this._textModelUri);
                await Global.SetModelLanguage(this.JSRuntime, this._textModel, language);
                await this.TextEditor!.SetModel(this._textModel);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError("Unable to set text editor language: {exception}", ex.ToString());
        }
    }

    /// <summary>
    /// Changes the value of the text editor
    /// </summary>
    /// <returns></returns>
    async Task SetTextEditorValueAsync()
    {
        var document = this.Get(state => state.DocumentText);
        if (this.TextEditor != null && !string.IsNullOrWhiteSpace(document) && this._hasTextEditorInitialized)
        {
            await this.TextEditor.SetValue(document);
        }
    }

    /// <summary>
    /// Copies to content of the Monaco editor to the clipboard
    /// </summary>
    /// <returns>A awaitable task</returns>
    public async Task OnCopyToClipboard()
    {
        if (this.TextEditor == null || !this._hasTextEditorInitialized) return;
        var text = await this.TextEditor.GetValue();
        if (string.IsNullOrWhiteSpace(text)) return;
        try
        {
            await this.JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            this.ToastService.Notify(new(ToastType.Success, "Copied to the clipboard!"));
        }
        catch (Exception ex)
        {
            this.ToastService.Notify(new(ToastType.Danger, "Failed to copy the definition to the clipboard."));
            this.Logger.LogError("Unable to copy to clipboard: {exception}", ex.ToString());
        }
    }
    #endregion

    /// <inheritdoc/>
    public override Task InitializeAsync()
    {
        this.DocumentText.SubscribeAsync(async (_) => {
            await this.SetTextEditorValueAsync();
        }, cancellationToken: this.CancellationTokenSource.Token);
        this.IsReadOnly.Subscribe((isReadOnly) =>
        {
            this.TextEditor?.UpdateOptions(new EditorUpdateOptions() { ReadOnly = isReadOnly });
        }, token: this.CancellationTokenSource.Token);
        this.MonacoEditorHelper.PreferredThemeChanged += OnPreferredThemeChangedAsync;
        return base.InitializeAsync();
    }

    /// <summary>
    /// Updates the editor theme
    /// </summary>
    /// <param name="newTheme">The new theme</param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected async Task OnPreferredThemeChangedAsync(string newTheme)
    {
        if (this.TextEditor != null && this._hasTextEditorInitialized) await this.TextEditor.UpdateOptions(new EditorUpdateOptions() { Theme = newTheme });
    }

    /// <summary>
    /// Disposes of the store
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the dispose of the store</param>
    protected override void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                if (this._textModel != null)
                {
                    this._textModel.DisposeModel();
                    this._textModel = null;
                }
                if (this.TextEditor != null)
                {
                    this.TextEditor.Dispose();
                    this.TextEditor = null;
                }
                this.MonacoEditorHelper.PreferredThemeChanged -= OnPreferredThemeChangedAsync;
            }
            this._disposed = true;
        }
    }

}