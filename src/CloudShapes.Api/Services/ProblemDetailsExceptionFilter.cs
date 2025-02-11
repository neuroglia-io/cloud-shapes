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