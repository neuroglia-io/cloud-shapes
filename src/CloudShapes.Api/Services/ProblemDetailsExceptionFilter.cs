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

using Microsoft.AspNetCore.Mvc.Filters;
using Neuroglia;

namespace CloudShapes.Api.Services;

/// <summary>
/// Represents an <see cref="IExceptionFilter"/> used to filter <see cref="ProblemDetailsException"/>s
/// </summary>
public class ProblemDetailsExceptionFilter 
    : IExceptionFilter
{

    /// <inheritdoc/>
    public virtual void OnException(ExceptionContext context)
    {
        if (context.Exception is not ProblemDetailsException ex) return;
        var result = new ObjectResult(ex.Problem)
        {
            StatusCode = ex.Problem.Status
        };
        context.Result = result;
        context.ExceptionHandled = true;
    }

}