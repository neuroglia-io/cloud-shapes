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

using Microsoft.Extensions.Logging;
using Neuroglia.Serialization;

namespace CloudShapes.Dashboard.Components.ProjectionDetailsStateManagement;

/// <summary>
/// Represents the <see cref="ComponentStore{TState}" /> of a <see cref="ProjectionDetails"/>
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="monacoEditorHelper">The service used to facilitate the Monaco editor interactions</param>
/// <param name="jsonSerializer">The The service used to serialize/deserialize objects to/from JSON</param>
/// <param name="yamlSerializer">The service used to serialize/deserialize objects to/from YAML</param>
public class ProjectionDetailsStore(ILogger<ProjectionDetailsStore> logger, ICloudShapesApiClient cloudShapesApi, IMonacoEditorHelper monacoEditorHelper, IJsonSerializer jsonSerializer, IYamlSerializer yamlSerializer)
    : ComponentStore<ProjectionDetailsState>(new())
{

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger<ProjectionDetailsStore> Logger { get; } = logger;

    #region Selectors
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.Projection"/> changes
    /// </summary>
    public IObservable<IDictionary<string, object>?> Projection => this.Select(state => state.Projection).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.SerializedProjection"/> changes
    /// </summary>
    public IObservable<string> SerializedProjection => this.Select(state => state.SerializedProjection).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.ProjectionType"/> changes
    /// </summary>
    public IObservable<ProjectionType?> ProjectionType => this.Select(state => state.ProjectionType).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.IsSaving"/> changes
    /// </summary>
    public IObservable<bool> IsSaving => this.Select(state => state.IsSaving).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.ProblemType"/> changes
    /// </summary>
    public IObservable<Uri?> ProblemType => this.Select(state => state.ProblemType).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.ProblemTitle"/> changes
    /// </summary>
    public IObservable<string> ProblemTitle => this.Select(state => state.ProblemTitle).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.ProblemDetail"/> changes
    /// </summary>
    public IObservable<string> ProblemDetail => this.Select(state => state.ProblemDetail).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.ProblemStatus"/> changes
    /// </summary>
    public IObservable<int> ProblemStatus => this.Select(state => state.ProblemStatus).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionDetailsState.ProblemErrors"/> changes
    /// </summary>
    public IObservable<IDictionary<string, string[]>> ProblemErrors => this.Select(state => state.ProblemErrors).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe computed <see cref="Neuroglia.ProblemDetails"/>
    /// </summary>
    public IObservable<ProblemDetails?> ProblemDetails => Observable.CombineLatest(
        ProblemType,
        ProblemTitle,
        ProblemStatus,
        ProblemDetail,
        ProblemErrors,
        (type, title, status, details, errors) =>
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }
            return new ProblemDetails(type ?? new Uri("unknown://"), title, status, details, null, errors, null);
        }
    );
    #endregion

    #region Setters
    /// <summary>
    /// Sets the state's <see cref="ProjectionDetailsState.Projection"/>
    /// </summary>
    /// <param name="projection">The new value</param>
    public void SetProjection(IDictionary<string, object>? projection)
    {
        Reduce(state => state with { Projection = projection });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionDetailsState.SerializedProjection"/>
    /// </summary>
    /// <param name="serializedProjection">The new value</param>
    public void SetProjectionValue(string serializedProjection)
    {
        Reduce(state => state with { SerializedProjection = serializedProjection });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionDetailsState.ProjectionType"/>
    /// </summary>
    /// <param name="projectionType">The new value</param>
    public void SetProjectionType(ProjectionType? projectionType)
    {
        Reduce(state => state with { ProjectionType = projectionType });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionDetailsState" /> <see cref="ProblemDetails"/>'s related data
    /// </summary>
    /// <param name="problem">The <see cref="ProblemDetails"/> to populate the data with</param>
    public void SetProblemDetails(ProblemDetails? problem)
    {
        Reduce(state => state with
        {
            ProblemType = problem?.Type,
            ProblemTitle = problem?.Title ?? string.Empty,
            ProblemStatus = problem?.Status ?? 0,
            ProblemDetail = problem?.Detail ?? string.Empty,
            ProblemErrors = problem?.Errors?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? []
        });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionDetailsState.IsSaving"/>
    /// </summary>
    /// <param name="isSaving">The new value</param>
    void SetSaving(bool isSaving)
    {
        Reduce(state => state with { IsSaving = isSaving });
    }
    #endregion

    #region Actions
    /// <summary>
    /// Saves the updated projection
    /// </summary>
    /// <returns></returns>
    public async Task SaveProjectionAsync()
    {
        var projection = Get(state => state.Projection);
        var projectionType = Get(state => state.ProjectionType);
        var serializedProjection = Get(state => state.SerializedProjection);
        if (projectionType == null || projection == null || string.IsNullOrWhiteSpace(serializedProjection)) return;
        SetProblemDetails(null);
        SetSaving(true);
        if (monacoEditorHelper.PreferredLanguage == PreferredLanguage.YAML) serializedProjection = yamlSerializer.ConvertToJson(serializedProjection);
        var jsonPatch = JsonPatch.FromDiff(jsonSerializer.SerializeToElement(projection)!.Value, jsonSerializer.SerializeToElement(jsonSerializer.Deserialize<IDictionary<string, object>>(serializedProjection))!.Value);
        var patch = jsonSerializer.Deserialize<Json.Patch.JsonPatch>(jsonPatch.RootElement);
        if (patch != null)
        {
            var projectionPatch = new Patch(PatchType.JsonPatch, jsonPatch);
            try
            {
                this.Logger.LogInformation($"jsonPatch: {jsonSerializer.SerializeToText(jsonPatch)}");
                this.Logger.LogInformation($"patch: {jsonSerializer.SerializeToText(patch)}");
                this.Logger.LogInformation($"projectionPatch: {jsonSerializer.SerializeToText(projectionPatch)}");
                this.Logger.LogInformation($"serializedProjection: {serializedProjection}");
                projection = (IDictionary<string, object>)(await cloudShapesApi.Projections.PatchAsync(new(projectionType.Name, projection["_id"].ToString()!, projectionPatch), CancellationTokenSource.Token));
            }
            catch (ProblemDetailsException ex)
            {
                this.SetProblemDetails(ex.Problem);
            }
            catch (Exception ex)
            {
                this.Logger.LogError("Unable to update resource: {exception}", ex.ToString());
            }
        }
        SetSaving(false);
    }
    #endregion

    /// <inheritdoc/>
    public override Task InitializeAsync()
    {
        return base.InitializeAsync();
    }

}