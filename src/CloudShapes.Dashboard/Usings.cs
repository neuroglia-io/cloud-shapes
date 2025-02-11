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

global using BlazorBootstrap;
global using CloudShapes.Api.Client;
global using CloudShapes.Api.Client.Services;
global using CloudShapes.Dashboard;
global using CloudShapes.Dashboard.Components;
global using CloudShapes.Dashboard.Layout;
global using CloudShapes.Dashboard.Services;
global using CloudShapes.Dashboard.Services.Interfaces;
global using CloudShapes.Dashboard.StateManagement;
global using CloudShapes.Data;
global using CloudShapes.Data.Models;
global using CloudShapes.Data.Serialization.Json;
global using CloudShapes.Integration;
global using Json.Schema;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
global using Microsoft.JSInterop;
global using moment.net;
global using moment.net.Models;
global using Neuroglia;
global using Neuroglia.Data;
global using Neuroglia.Eventing.CloudEvents;
global using Neuroglia.Reactive;
global using Neuroglia.Serialization;
global using Pluralize.NET;
global using System.ComponentModel;
global using System.Globalization;
global using System.Reactive.Linq;
global using System.Reactive.Subjects;
global using System.Text.Json;
global using System.Text.Json.Serialization;
