using System.Net;
using Newtonsoft.Json;
using Shared.Exceptions;
using Shared.Middleware;

namespace Dissertation_API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        this._next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await this._next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        var problem = new CustomProblemDetails { Instance = env };

        switch (ex)
        {
            case BadRequestException badRequestException:
                statusCode = HttpStatusCode.BadRequest;
                problem.Title = badRequestException.Message;
                problem.Status = (int)statusCode;
                problem.Detail = badRequestException.InnerException?.Message;
                problem.Type = nameof(BadRequestException);
                problem.Errors = badRequestException.ValidationErrors;
                break;
            case NotFoundException notFound:
                statusCode = HttpStatusCode.NotFound;
                problem.Title = notFound.Message;
                problem.Status = (int)statusCode;
                problem.Type = nameof(NotFoundException);
                problem.Detail = notFound.InnerException?.Message;
                break;
            case UnauthorizedException unAuthorized:
                statusCode = HttpStatusCode.Unauthorized;
                problem.Title = unAuthorized.Message;
                problem.Status = (int)statusCode;
                problem.Type = nameof(NotFoundException);
                problem.Detail = unAuthorized.InnerException?.Message;
                break;
            default:
                problem.Title = ex.Message;
                problem.Status = (int)statusCode;
                problem.Type = nameof(HttpStatusCode.InternalServerError);
                problem.Detail = ex.StackTrace;
                break;
        }

        httpContext.Response.StatusCode = (int)statusCode;
        var logMessage = JsonConvert.SerializeObject(problem);
        this._logger.LogError(logMessage);
        await httpContext.Response.WriteAsJsonAsync(problem);
    }
}