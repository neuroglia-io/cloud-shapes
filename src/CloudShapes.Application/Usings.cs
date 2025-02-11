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

global using CloudShapes.Application.Configuration;
global using CloudShapes.Application.Services;
global using CloudShapes.Data;
global using CloudShapes.Integration.Events.Projections;
global using CloudShapes.Data.Models;
global using Json.Schema;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using MongoDB.Bson;
global using MongoDB.Bson.IO;
global using MongoDB.Bson.Serialization;
global using MongoDB.Driver;
global using Neuroglia;
global using Neuroglia.Data.Expressions;
global using Neuroglia.Data.Expressions.Services;
global using Neuroglia.Data.PatchModel.Services;
global using Neuroglia.Eventing.CloudEvents;
global using Neuroglia.Eventing.CloudEvents.Infrastructure.Services;
global using Neuroglia.Mediation;
global using Neuroglia.Serialization;
global using Pluralize.NET;
global using System.Net;
global using System.Net.Mime;
global using System.Runtime.CompilerServices;
global using System.Text.RegularExpressions;
