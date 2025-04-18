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

namespace CloudShapes.Dashboard.Pages.ProjectionTypes.Indexes;

/// <summary>
/// Represents the store the the create projection type view
/// </summary>
/// <param name="logger">The service used to perform logging</param>
/// <param name="cloudShapesApi">The service used to interact with the Cloud Shapes API</param>
/// <param name="monacoEditorHelper">The service used to to facilitate the Monaco Indexes configuration</param>
/// <param name="jsonSerializer">The service used to serialize/deserialize data to/from JSON</param>
/// <param name="yamlSerializer">The service used to serialize/deserialize data to/from YAML</param>
public class ProjectionTypeIndexesStore(ILogger<ProjectionTypeIndexesStore> logger, ICloudShapesApiClient cloudShapesApi, IMonacoEditorHelper monacoEditorHelper, IJsonSerializer jsonSerializer, IYamlSerializer yamlSerializer)
    : ComponentStore<ProjectionTypeIndexesState>(new())
{
    #region Selectors
    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeIndexesState.Status"/> changes
    /// </summary>
    public IObservable<string> Status => this.Select(state => state.Status).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeIndexesState.ProjectionTypeName"/> changes
    /// </summary>
    public IObservable<string?> ProjectionTypeName => this.Select(state => state.ProjectionTypeName).DistinctUntilChanged();

    /// <summary>
    /// Gets an <see cref="IObservable{T}"/> used to observe <see cref="ProjectionTypeIndexesState.ProjectionType"/> changes
    /// </summary>
    public IObservable<ProjectionType> ProjectionType => this.Select(state => state.ProjectionType)
        .Where(projectionType => projectionType != null)
        .DistinctUntilChanged()!;
    #endregion

    #region Setters
    /// <summary>
    /// Sets the state's <see cref="ProjectionTypeIndexesState.ProjectionType"/>
    /// </summary>
    /// <param name="projectionType">The new value</param>
    protected void SetProjectionType(ProjectionType projectionType)
    {
        Reduce(state => state with
        {
            ProjectionType = projectionType
        });
    }

    /// <summary>
    /// Sets the state's <see cref="ProjectionTypeIndexesState.ProjectionTypeName"/>
    /// </summary>
    /// <param name="projectionTypeName">The <see cref="ProjectionType"/>'s name</param>
    public void SetProjectionTypeName(string projectionTypeName)
    {
        Reduce(state => state with
        {
            ProjectionTypeName = projectionTypeName,
        });
    }
    #endregion

    #region Actions
    #endregion

    /// <inheritdoc/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        ProjectionTypeName
            .Where(name => !string.IsNullOrEmpty(name))
            .SubscribeAsync(async name => {
                var projectionType = await cloudShapesApi.ProjectionTypes.GetAsync(name!, CancellationTokenSource.Token);
                SetProjectionType(projectionType);
            }, CancellationTokenSource.Token);
    }
}